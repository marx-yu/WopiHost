## Introduction

This repository contains a Wopi Host demo.
work with Office Online Server 2016(OOS). 
Support DOCX Editing, and also PPTX,XLSX.
Welcome any contribution, and discussion of supporting coauthoring

## Requirements

Requires Office Online 2016 Server(preview). 
*No need Microsoft.CobaltCore.dll assembly.*

## Known Issues

Doesn't support coauthoring.

## Usage & Examples

#### Deploy OOS
[Deploy Guideline](https://docs.microsoft.com/en-us/officeonlineserver/deploy-office-online-server)   
Set editing enabled:  Set-OfficeWebAppsFarm -EditingEnabled:$true

#### Prepare Wopi Host
1.Set real local docs path as code below in WopiCobalthost\program.cs
```
// docsPath parameter may change to the real local path that save demo documents(word or excel file)
CobaltServer svr = new CobaltServer(@"D:\\wopi-docs");
```
2.Set real hostname or domain as code below in WopiCobaltHost\CobaltServer.cs
```
// localhost may change to the real hostname or IP
m_listener.Prefixes.Add(String.Format("http://localhost:{0}/wopi/", m_port));
```
You can use FQDN as hostname for remote access.   
If you have no FQDN, you can use localhost and use [ngrok](https://ngrok.com/download) as proxy:ngrok.exe http -host-header=localhost 8080

3.Build and start this wopihost program   
Assume the demo doc file name is word.docx   
Now you can access http://[wopi.hostname]/wopi/files/word.docx, and get the CheckFileInfo response

#### Open OOS website
1.URLEncode wopihost address to:http%3a%2f%2fwopi.hostname%2fwopi%2ffiles%2fword.docx   
2.Use URL below to open OOS editing web page   
For word.docx:   
http://[OOS.hostname]/we/WordEditorFrame.aspx?WOPISrc=http%3a%2f%2fwopi.hostname%2fwopi%2ffiles%2fword.docx&access_token=token&ui=zh-CN   
For ppt.pptx:   
http://[OOS.hostname]/p/PowerPointFrame.aspx?PowerPointView=ReadingView&WOPISrc=http%3a%2f%2fwopi.hostname%2fwopi%2ffiles%2fppt.pptx&access_token=token&ui=zh-CN   
For Book1.xlsx   
http://[OOS.hostname]/x/_layouts/xlviewerinternal.aspx?WOPISrc=http%3a%2f%2fwopi.hostname%2fwopi%2ffiles%2fBook1.xlsx&access_token=token&ui=zh-CN
