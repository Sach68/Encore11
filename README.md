# Encore11

**Un assistant en français pour passer de Windows 10 à Windows 11 sur un PC qui ne remplit pas toutes les exigences matérielles.**

Je suis passionné d’informatique et je veux lutter contre l’obsolescence programmée. J’ai créé ce lanceur pour aider à prolonger la durée de vie de PC qui fonctionnent encore.

## Télécharger et utiliser

1. Dans l’onglet [Releases](../../releases), téléchargez `LanceurWin11.exe` sous **Assets**.
2. Lancez le programme depuis le Windows 10 que vous voulez mettre à niveau. Cliquez sur **Télécharger chez Microsoft**. Dans le navigateur, choisissez l’ISO officielle **Windows 11 x64** dans la même langue que votre installation de Windows.
3. Revenez au lanceur, cliquez sur **Choisir l’ISO téléchargée** et sélectionnez le fichier. Le lanceur monte l’ISO et ouvre Windows Setup. Windows peut demander les droits administrateur à cette étape.
4. Si vous souhaitez garder votre environnement actuel, vérifiez que le récapitulatif de Windows Setup affiche **« Conserver les fichiers personnels et les applications »** avant de cliquer sur **Installer**. Si cette option manque, arrêtez la procédure.


## Pourquoi l’installateur affiche « Windows Server » ?

Le lanceur ouvre `setup.exe` avec le paramètre `/Product Server`. L’interface peut afficher « Windows Server », tandis que l’édition proposée provient de l’ISO. Avec l’ISO officielle de Windows 11, l’installation vise une édition de Windows 11 classique. Ce parcours peut échouer selon le PC et la version de l’ISO. Microsoft ne garantit pas les mises à jour sur les appareils non pris en charge. [Assistance Microsoft](https://support.microsoft.com/en-us/windows/experience/compatibility/windows-11-on-devices-that-don-t-meet-minimum-system-requirements)

## Résultats des tests
Dans tous les cas testés, la mise à niveau vers Windows 11 s’est déroulée sans problème. Après le redémarrage, le système fonctionnait normalement, comme après une installation sur un PC compatible.
## Signaler un résultat

Ouvrez une [issue GitHub](../../issues) et indiquez le modèle du PC ou le type de VM, la version de Windows de départ, la version de l’ISO et le résultat. Si l’installation échoue, indiquez l’étape et le message affiché. Masquez les données personnelles et les clés de produit sur vos captures.

