# The Mad Archivist

Desktop application for managing your archives



# Vibe-code Commands

- Update the title to show app title and page in the form "App name - Page name"

## Controls

- Create a user control. It will display a list of files and folders, given a folder path place it in the appropriate location
- Create a user control. Given a folder path, it will display a tree view of child files and folders
- Add a status bar at the bottom of the app
- Do not add a page to the navigation stack if the current page is the last page pushed onto the stack

## Toolbars

- Rename TopCommandBar to MainCommandBar
- Add a new toolbar called File. Make it dockable
- Add a Settings button to the right of the menu bar. Use the icon only. Right justify it


## Layout

- Add Home page
- Add a MenuBar at the top of the app
- Add a CommandBar at the top of the app, below the MenuBar
- Move the CommandBar to the left. Add a toggle to the settings to allow the user to move between left and right.

- Add back and forward Navigation buttons to the command bar, to navigate to visited pages
- Remember the last position and size of the app when it closes

## Settings

- Add a page called Settings. Navigate to this page when a user clicks on Settings button
- Add a toggle to Settings. When on, this will activate dark mode
- Add System Default as an option for the Dark mode setting


## Library - FolderTools

- Add a method in AppTools.FolderTools.cs called MapDrive. Given a folder path, a drive letter, and a nama, it will create a mapped drive. Return error code if operation fails.
- Add a method in AppTools.FolderTools.cs called UnmapDrive. Given a drive letter, un map the drive. Return a status flag.
- Add a method in AppTools.FolderTools.cs called UpdateFolderIcon. It will take a path to an Icon file and a folder path. When called, update the folder icon with the supplied icon
- 

## Library - FileTools

- Add a method in AppTools.FileTools.cs called SaveIcon. Given an Icon, and a file path, save the icon to file
- Add a method to AppTools.FileTools.cs called IsIdentical. Given 2 file paths, check if the files contents are the same. Optimize method for speed.

## Library - EncryptionTools

- Add a method to AppTools.EncryptionTools.cs called EncryptFile. It will encrypt a file specified by an input file path and save it to a specified location
- Add a method to AppTools.EncryptionTools.cs called DecryptFile. It will decrypt a file specified by an input file path and save it to a specified location



## Library - ImageTools

- Add a method in Tools.ImageTools.ca called ToIcon. Given the path to an image, create a windows icon. The method will return this icon

## Main Page



## Help - About

- Add an about page to the app. Navigate to it when a user presses the About menu command



