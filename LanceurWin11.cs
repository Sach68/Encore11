using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

internal static class Lanceur
{
    private const string PageMicrosoft = "https://www.microsoft.com/fr-fr/software-download/windows11";

    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        using (var fenetre = new Form())
        {
            fenetre.Text = "Lanceur Windows 11";
            fenetre.ClientSize = new Size(640, 466);
            fenetre.StartPosition = FormStartPosition.CenterScreen;
            fenetre.FormBorderStyle = FormBorderStyle.FixedDialog;
            fenetre.MaximizeBox = false;
            fenetre.AutoScaleMode = AutoScaleMode.Dpi;
            fenetre.BackColor = Color.White;
            fenetre.ForeColor = Color.FromArgb(25, 36, 46);
            fenetre.Font = new Font("Segoe UI", 10);

            var titre = new Label {
                Left = 24, Top = 20, Width = 592, Height = 47,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Text = "Lanceur Windows 11"
            };
            var presentation = new Label {
                Left = 26, Top = 76, Width = 584, Height = 76,
                Text = "Je suis passionné d’informatique et je veux lutter contre l’obsolescence programmée. J’ai créé ce lanceur pour aider à prolonger la durée de vie des PC qui fonctionnent encore, même quand ils ne répondent pas à toutes les exigences de Windows 11."
            };
            var instructions = new Label {
                Left = 26, Top = 157, Width = 584, Height = 86,
                Text = "1. Téléchargez l’ISO officielle de Windows 11 x64 depuis Microsoft.\n" +
                    "2. Cliquez sur « Choisir l’ISO téléchargée » et sélectionnez le fichier.\n" +
                    "3. Vérifiez les options de conservation dans l’installateur."
            };
            var modeServeur = new Label {
                Left = 26, Top = 253, Width = 584, Height = 109,
                Padding = new Padding(12, 10, 12, 8),
                BackColor = Color.FromArgb(234, 244, 249),
                Text = "Pourquoi « Windows Server » ?\n" +
                    "Le lanceur ouvre l’installateur en mode Windows Server. C’est l’ISO choisie qui contient l’édition à installer. Avec l’ISO officielle de Windows 11, l’installation vise Windows 11 classique. Le résultat dépend de la compatibilité du PC."
            };
            var telecharger = new Button {
                Left = 26, Top = 379, Width = 278, Height = 42, Text = "Télécharger chez Microsoft"
            };
            var importer = new Button {
                Left = 320, Top = 379, Width = 290, Height = 42, Text = "Choisir l’ISO téléchargée",
                FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(0, 86, 124), ForeColor = Color.White,
                UseVisualStyleBackColor = false
            };
            var etat = new Label { Left = 26, Top = 430, Width = 584, Height = 25 };
            fenetre.Controls.AddRange(new Control[] { titre, presentation, instructions, modeServeur, telecharger, importer, etat });

            telecharger.Click += (sender, args) =>
            {
                try { Process.Start(PageMicrosoft); }
                catch (Exception erreur) { MessageBox.Show(fenetre, erreur.Message, "Navigateur indisponible"); }
            };
            importer.Click += async (sender, args) =>
            {
                string chemin;
                using (var choix = new OpenFileDialog())
                {
                    choix.Title = "Choisir l’ISO de Windows 11";
                    choix.Filter = "Image disque Windows (*.iso)|*.iso";
                    if (choix.ShowDialog(fenetre) != DialogResult.OK)
                        return;
                    chemin = choix.FileName;
                }

                if (MessageBox.Show(fenetre,
                    "Le programme va monter l’ISO sélectionnée et ouvrir son fichier setup.exe. Dans l’installateur, vérifiez que « Conserver les fichiers personnels et les applications » est sélectionné.",
                    "Ouvrir Windows Setup", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK)
                    return;

                importer.Enabled = false;
                etat.Text = "Montage de l’ISO et ouverture de l’installateur…";
                try
                {
                    await Task.Run(() => DemarrerSetup(chemin));
                    fenetre.Close();
                }
                catch (Exception erreur)
                {
                    etat.Text = "";
                    MessageBox.Show(fenetre, erreur.Message, "Impossible d’ouvrir Windows Setup",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    importer.Enabled = true;
                }
            };

            Application.Run(fenetre);
        }
    }

    private static void DemarrerSetup(string chemin)
    {
        string encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(Script));
        var options = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory,
            @"WindowsPowerShell\v1.0\powershell.exe"), "-NoProfile -NonInteractive -EncodedCommand " + encoded)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            StandardErrorEncoding = Encoding.UTF8
        };
        options.EnvironmentVariables["WIN11_LAUNCHER_ISO"] = chemin;
        using (var process = Process.Start(options))
        {
            string erreur = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0)
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(erreur)
                    ? "Windows Setup n’a pas pu démarrer." : erreur.Trim());
        }
    }

    private const string Script = @"
$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'
[Console]::OutputEncoding = [Text.Encoding]::UTF8
try {
    $path = $env:WIN11_LAUNCHER_ISO
    $iso = Get-DiskImage -ImagePath $path
    $mountedHere = -not $iso.Attached
    try {
        if ($mountedHere) { $iso = Mount-DiskImage -ImagePath $path -PassThru }
        foreach ($volume in ($iso | Get-Volume | Where-Object DriveLetter)) {
            $setup = Join-Path ($volume.DriveLetter + ':\') 'setup.exe'
            if (!(Test-Path -LiteralPath $setup -PathType Leaf)) { continue }
            Start-Process -FilePath $setup -ArgumentList '/Product', 'Server' -Verb RunAs
            exit 0
        }
        throw 'setup.exe est introuvable sur cette ISO.'
    } catch {
        if ($mountedHere) { Dismount-DiskImage -ImagePath $path -ErrorAction SilentlyContinue }
        throw
    }
} catch {
    [Console]::Error.WriteLine($_.Exception.Message)
    exit 1
}
";
}
