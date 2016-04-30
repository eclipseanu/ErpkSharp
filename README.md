# ErpkSharp

## Installation

1. Clone this repository on your computer.
2. In Visual Studio, click `File -> Add -> Existing Project...`
3. Point the location of [`Erpk/Erpk.csproj`](Erpk/Erpk.csproj)

## Create client

```csharp
using Erpk.Http;

// Pass eRepublik e-mail and password.
var session = new Session("foo@bar.com", "qwerty");
var client = new Client(session)
{
    UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0"
};
```

## Automatically save session cookies on disk

```csharp
var manager = new SessionManager("sessions"); // Directory where sessions will be stored.
var session = manager.OpenOrCreate("foo@bar.com", "qwerty"); // Open saved session or create new one.
session.Modified += () => manager.Save(session); // Save session when modified.

var client = new Client(session);
```

## Initialize module

I assume that you already have created client object.

```csharp
using Erpk.Modules;

var module = client.Resolve<MilitaryModule>();
//var module = client.Resolve<MarketModule>();
//var module = client.Resolve<TrainModule>();
//var module = client.Resolve<WorkModule>();
//var module = client.Resolve<DonationsModule>();
```

Full list of modules is available [here](Erpk/src/Modules). Please don't use `LoginModule`, it's used only internally and you don't need to care about it.

## Make HTTP requests

```csharp
// Will create GET request to http://www.erepublik.com/en/military/campaigns-new/
var req = client.Get("military/campaigns-new/");
var res = await req.Send();
```
You don't have to worry about login, it's done automatically.

```csharp
// Will create POST request to http://www.erepublik.com/en/main/travel
var req = client.Post("main/travel").CSRF();
req.Form.Add("check", "moveAction");
req.Form.Add("toCountryId", 41);
req.Form.Add("inRegionId", 532);
req.Form.Add("battleId", 76608);

var res = await req.Send();
```

## Code samples
### Work in companies
```csharp
var mod = client.Resolve<WorkModule>();

// Work as employee.
var resultEmployee = await mod.WorkAsEmployee();

// Work overtime.
var resultOvertime = await mod.WorkOvertime();

// Work as manager.
var companiesPage = await mod.MyCompaniesPage();
var queue = companiesPage
    .Companies
    .Where(c => !c.already_worked) // Ignore companies where you've already worked.
    .Where(c => c.industry_token != "HOUSE") // You can't work as manager in house companies.
    .Select(c => new ProductionTask(c.id, true));
    
var resultManager = await mod.WorkAsManager(queue);
```
