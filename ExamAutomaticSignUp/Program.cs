using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


var configFilePath = "../../../config.txt";

string username, password, url;

GetInput(out username, out password, out url);

IWebDriver driver = new ChromeDriver();
driver.Navigate().GoToUrl(url);

if (NeedToLogIn(driver))
{
    LogIn(driver, username, password);
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

    if (fileLines.Length < 3)
    {
        username = WaitForUsername();
        password = WaitForPassword();
        url = WaitForUrl();
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

bool NeedToLogIn(IWebDriver driver)
{
    var loginButton = driver.FindElements(By.Id("login"));
    if (loginButton.Count > 0) return true;

    return false;
}

void LogIn(IWebDriver driver, string username, string password)
{
    var usernameInput = driver.FindElement(By.Id("meno"));
    var passwordInput = driver.FindElement(By.Id("heslo"));
    var loginButton = driver.FindElement(By.Id("login"));
    usernameInput.SendKeys(username);
    passwordInput.SendKeys(password);
    loginButton.Click();
}
