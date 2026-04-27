# The Mad Archivist

Desktop application for managing your files

# Commands

- Update the title to show app title and page in the form "App name - Page name"

## Controls

- Create a user control. It will display a list of files and folders, given a folder path place it in the appropriate location
- Create a user control. Given a folder path, it will display a tree view of child files and folders

## Layout

- Add Home page
- Add a MenuBar at the top of the app
- Add a CommandBar at the top of the app, below the MenuBar
- Add back and forward Navigation buttons to the command bar, to navigate to visited pages

## Settings

- Add a page called Settings. Navigate to this page when a user clicks on Settings button
- Add a toggle to Settings. When on, this will activate dark mode

## Library - FolderTools

- Add a method in Tools.FolderTools.ca called MapDrive. Given a folder path, a drive letter, and a nama, it will create a mapped drive. Return error code if operation fails.
- Add a method in Tools.FolderTools.ca called UnmapDrive. Given a drive letter, un map the drive. Return a status flag.
- Add a method in Tools.FolderTools.ca called UpdateFolderIcon. It will take a path to an Icon file and a folder path. When called, update the folder icon with the supplied icon
- 

## Library - FileTools

- Add a method in Tools.FileTools.ca called SaveIcon. Given an Icon, and a file path, save the icon to file

## Library - ImageTools

- Add a method in Tools.ImageTools.ca called ToIcon. Given the path to an image, create a windows icon. The method will return this icon

## Help - About

- Add an about page to the app. Navigate to it when a user presses the About menu command



