using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using System.Globalization;


const string configFilePath = "../../../config.txt";

string username, password, url, dateStr;
int delayTimeInMinutes;

GetInput(out username, out password, out url, out delayTimeInMinutes);

IWebDriver driver = new ChromeDriver();
driver.Navigate().GoToUrl(url);

if (NeedToLogIn())
{
    await LogIn();
    Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute} - Successfully logged in.");
    driver.Navigate().GoToUrl(url);
}

await Task.Delay(1000);

while (true)
{
    IWebElement table = driver.FindElement(By.CssSelector("table.SRH-inp2"));

    ClickCookieBanner();

    var rows = table.FindElements(By.TagName("tr"));
    foreach (var row in rows)
    {
        var cells = row.FindElements(By.TagName("td"));

        if (cells.Count == 0)
            continue;

        string rowDate = cells[0].Text.Trim();

        if (rowDate == dateStr)
        {
            int capacity = int.Parse(cells[3].Text);
            int alreadyRegistered = int.Parse(cells[4].Text);
            if (alreadyRegistered == capacity)
            {
                Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute} - No available slots for the selected date.");
                break;
            }

            ClickRegisterButton(cells);

            return;
        }
    }
    
    await Task.Delay(delayTimeInMinutes * 60 * 1000);
}

void ClickRegisterButton(IList<IWebElement> cells)
{
    try
    {
        IWebElement registerLink = cells.Last().FindElement(
            By.XPath(".//a[img[@title='Prihlásenie na termín']]")
        );

        registerLink.Click();

        Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute} - Successfully clicked register button.");
    }
    catch (NoSuchElementException)
    {
        Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute} - Register button not found.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute} - Failed to click register button: {ex.Message}");
    }
}

void ClickCookieBanner()
{
    try
    {
        var cookiesButton = driver.FindElement(By.CssSelector("#hlaska-cookies button, #hlaska-cookies a"));
        cookiesButton.Click();
        Console.WriteLine($"{DateTime.Now.Hour}:{DateTime.Now.Minute} - Cookies banner closed.");
    }
    catch
    {
        return;
    }
}

bool NeedToLogIn()
{
    var loginButton = driver.FindElements(By.Id("login"));
    if (loginButton.Count > 0) return true;

    return false;
}

async Task LogIn()
{
    var usernameInput = driver.FindElement(By.Id("meno"));
    var passwordInput = driver.FindElement(By.Id("heslo"));
    var loginButton = driver.FindElement(By.Id("login"));
    usernameInput.SendKeys(username);
    passwordInput.SendKeys(password);
    loginButton.Click();
    await Task.Delay(1000);
}

void GetInput(out string username, out string password, out string url, out int delayTimeInMinutes)
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

    if (fileLines.Length != 5)
    {
        Console.WriteLine("Config file not found or invalid. Please enter the required information:");
        username = WaitForUsername();
        password = WaitForPassword();
        url = WaitForUrl();
        dateStr = WaitForDate();
        delayTimeInMinutes = WaitForDelayTimeInMinutes();
    }
    else
    {
        url = fileLines[0].Trim(); // url
        username = fileLines[1].Trim(); // username
        password = fileLines[2].Trim(); // password
        dateStr = fileLines[3].Trim(); // date
        delayTimeInMinutes = int.Parse(fileLines[4].Trim()); // delay time in minutes
    }
}

int WaitForDelayTimeInMinutes()
{
    Console.WriteLine("Enter delay time in minutes:");
    int delayTimeInMinutes;
    while (!int.TryParse(Console.ReadLine(), out delayTimeInMinutes))
    {
        Console.WriteLine("Invalid input. Please enter a valid number of minutes:");
    }
    return delayTimeInMinutes;
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
        calendar.ToDateTime(yearInt, monthInt, dayInt, hourInt, minuteInt, 0, 0);
    } catch { 
        Console.WriteLine("Invalid date. Please enter again.");
        return WaitForDate();
    }

    return dateStr;
}
