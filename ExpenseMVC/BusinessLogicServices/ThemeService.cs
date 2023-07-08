namespace ExpenseMVC.BusinessLogicServices;

public class ThemeService
{
    public IDictionary<string,string> Themes { get; private set; }  = new Dictionary<string, string>() {
        { "minty", "bootstrap-icons.min.css"},
        { "materia", "bootstrap.materia.min.css" },
        { "quartz", "bootstrap-quartz.min.css" }
    };
    
}