# Asynclass

Asynclass is a library that allows the developer to create projects using asynchronous constructors, following a `Config-Init-Catch` pattern. The main objective is to expand asynchronous driven development to cover OOP concepts concisely.

## Basic usage

With Asynclass, it is possible to create async constructors that can be called using the `new` keyword. This allows asynchronous methods and features to be executed directly on the object's construction, acting as a "sugar" to reduce code, improve readability and make new ways to develop programs.

**Async classes can be called as the following:**

```cs
var currentWeather = await new WeatherInfo(DateTime.Now);

Console.WriteLine($"The current temperature is {currentWeather.Temperature}"); 
Console.WriteLine($"The current wind speed is {currentWeather.WindSpeed}"); 
```
```cs
using Asynclass;
using System;

class WeatherInfo : Async<WeatherInfo> 
{
    public double Temperature { get; set; }
    public double WindSpeed { get; set; }
    
    public WeatherInfo(DateTime dateTime) 
    {
        Init(async () => 
        {
            Temperature = await _externalService.GetTemperature();
            WindSpeed = await _externalService.GetWindSpeed();
        });
    }
}
```
