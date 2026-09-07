using System;

namespace MultiEditorPCC;

public class AppMsg
{

    public Status Status { get; set; }

    public String Title { get; set; }
    public String Content { get; set; }

    public String StatusString { get; set; }

    public AppMsg(Status status = Status.OK, String title = "", String content = "")
    {
        Status = status;
        Title = title;
        Content = content;

        switch (Status)
        {
            case Status.OK: StatusString = "Success"; break;
            case Status.Info: StatusString = "Information"; break;
            case Status.Warning: StatusString = "Warning"; break;
            case Status.Error: StatusString = "Error"; break;
        }
    }

}


public enum Status
{
    OK,
    Info,
    Warning,
    Error
}