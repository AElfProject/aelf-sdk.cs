# AElf-Dotnet-SDK

DotNet Sdk for AElf.

## Introduction

This is a .NET SDK library, written in C#, used to communicate with the AElf API.

### Basic usage

```c#
private const int TimeOut = 60;
private const int RetryTimes = 3;
private const string RequestUrl = "Http://127.0.0.1:8100";

var AElfClient = new AElfService(new HttpService(TimeOut, RetryTimes), RequestUrl);
```

### Test

You need to run a local or remote AElf node to run the unit test successfully.