using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Globalization;


const string configFilePath = "../../../config.txt";

string username, password, url, dateStr;

GetInput(out username, out password, out url);

IWebDriver driver = new ChromeDriver();
driver.Navigate().GoToUrl(url);

if (NeedToLogIn(driver))
{
    await LogIn(driver, username, password);
    driver.Navigate().GoToUrl(url);
}

void GetInput(out string username, out string password, out string url)
{
    string[] fileLines;
    try
    {
        fileLines = File.ReadAllLines(configFilePath);
    }
    catch (Exception)
    {
        fileLines = new string[0];
    }

    if (fileLines.Length != 4)
    {
        Console.WriteLine("Config file not found or invalid. Please enter the required information:");
        username = WaitForUsername();
        password = WaitForPassword();
        url = WaitForUrl();
        dateStr = WaitForDate();
    }
    else
    {
        url = fileLines[0].Trim(); // url
        username = fileLines[1].Trim(); // username
        password = fileLines[2].Trim(); // password
    }
}

string WaitForUsername()
{
    Console.WriteLine("Enter username:");
    string username = Console.ReadLine();
    while (username == null || username == "")
    {
        Console.WriteLine("Username cannot be empty. Please enter again:");
        username = Console.ReadLine();
    }
    return username;
}

string WaitForPassword()
{
    Console.WriteLine("Enter password:");
    string password = Console.ReadLine();
    while (password == null || password == "")
    {
        Console.WriteLine("Password cannot be empty. Please enter again:");
        password = Console.ReadLine();
    }
    return password;
}

string WaitForUrl()
{
    Console.WriteLine("Enter URL:");
    string url = Console.ReadLine();
    while (url == null || url == "")
    {
        Console.WriteLine("URL cannot be empty. Please enter again:");
        url = Console.ReadLine();
    }
    return url;
}

// did not check if it works, who cares, nobody will use this, not even me, just fun
string WaitForDate()
{
    Console.WriteLine("Enter date (dd.MM.yyyy / hh:mm):");

    //012345678901234567
    //18.05.2026 / 08:00
    string dateStr = Console.ReadLine();
    string day = dateStr.Substring(0, 2);
    string month = dateStr.Substring(3, 2);
    string year = dateStr.Substring(6, 4);
    string hour = dateStr.Substring(13, 2);
    string minute = dateStr.Substring(16, 2);

    int dayInt, monthInt, yearInt, hourInt, minuteInt;
    if (!int.TryParse(day, out dayInt) || !int.TryParse(month, out monthInt) || !int.TryParse(year, out yearInt) || !int.TryParse(hour, out hourInt) || !int.TryParse(minute, out minuteInt))
    {
        Console.WriteLine("Invalid date format. Please enter again.");
        return WaitForDate();
    }

    Calendar calendar = CultureInfo.CurrentCulture.Calendar;
    try
    {
        calendar.ToDateTime(yearInt, monthInt, dayInt, hourInt, minuteInt, 0, 0); // this will throw an exception if the date is not valid (e.g. 30.02.2024)
    } catch { 
        Console.WriteLine("Invalid date. Please enter again.");
        return WaitForDate();
    }

    return dateStr;
}

bool NeedToLogIn(IWebDriver driver)
{
    var loginButton = driver.FindElements(By.Id("login"));
    if (loginButton.Count > 0) return true;

    return false;
}

async Task LogIn(IWebDriver driver, string username, string password)
{
    var usernameInput = driver.FindElement(By.Id("meno"));
    var passwordInput = driver.FindElement(By.Id("heslo"));
    var loginButton = driver.FindElement(By.Id("login"));
    usernameInput.SendKeys(username);
    passwordInput.SendKeys(password);
    loginButton.Click();
    await Task.Delay(1000);
}
