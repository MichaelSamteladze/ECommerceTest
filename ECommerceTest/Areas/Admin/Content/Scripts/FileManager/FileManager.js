var SelectedFile = null;

$(function () {
    if (IsTinyMce) {
        DevexFileManager.GetToolbarItemByCommandName('ChoosePictureButton').SetVisible(false);
    }
    else {
        DevexFileManager.GetToolbarItemByCommandName('InsertPictureButton').SetVisible(false);
    }
});



function OnFileManagerSelectionChanged(s, e) {
    SelectedFile = s.GetSelectedFile();
    s.GetToolbarItemByCommandName('InsertPictureButton').SetEnabled(SelectedFile != null);
    s.GetToolbarItemByCommandName('ChoosePictureButton').SetEnabled(SelectedFile != null);
}

function OnFileManagerCustomCommand(s, e) {

    if (SelectedFile == null) {
        switch (e.commandName) {
            case "Refresh": {
                DevexFileManager.Refresh();
                break;
            }
        }
    }
    else {
        if (IsTinyMce) {
            InsertIntoTinyMce(e.commandName)
        }
        else if (e.commandName == 'ChoosePictureButton')
        {
            window.parent.OnFileManagerFileSelected(SelectedFile.name, FileManagerFolderHttpPath + SelectedFile.name);
        }
    }
}

function InsertIntoTinyMce(commandName)
{
    var Html = null;
    switch (commandName) {
        case "Default": {
            var Html = '<img src="' + FileManagerFolderHttpPath + SelectedFile.name + '" alt="" />';
            break;
        }
        case "LeftAligned": {
            var Html = '<img style="float:left; margin-right:10px;" src="' + FileManagerFolderHttpPath + SelectedFile.name + '"  alt="" />';
            break;
        }
        case "RightAligned": {
            var Html = '<img style="float:right; margin-left:10px;" src="' + FileManagerFolderHttpPath + SelectedFile.name + '"  alt="" />';
            break;
        }
        case "CenterAligned": {
            var Html = '<div style="width:100%; text-align:center"><img src="' + FileManagerFolderHttpPath + SelectedFile.name + '"  alt="" /></div>';
            break;
        }
    }
    
    if (Html != null) {        
        //parent.tinymce.activeEditor.insertContent(Html);
        //parent.tinymce.activeEditor.windowManager.close();

        parent.tinymce.activeEditor.insertContent(Html);
        parent.tinymce.activeEditor.windowManager.close();
    }
}

function OnFileManagerFilesUploaded(s, e) {
    setTimeout(function () {
        DevexFileManager.Refresh();
    }, 1000);    
}