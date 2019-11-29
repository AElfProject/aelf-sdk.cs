# AElf-WebApp-SDK

WebApp sdk for AElf.

## Introduction

This is the AElf CSharp WebSdk.

### Basic usage

```c#
private const string Url = "127.0.0.1:8001";
IWebAppService WebAppService = AElfWebAppClient.GetClientByUrl(Url);
```

### Test

You need to run a local or remote AElf node to run the unit test successfully.