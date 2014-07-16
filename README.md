### Introduction

This repository contains a Wopi Host demo.
With Cobalt support for Office Web Apps. 
Support DOCX Editing, and also PPTX,XLSX.

### Requirements

Requires Microsoft.CobaltCore.dll assembly from Office Web Apps server. 

### Known Issues

Doesn't support coauthoring.

### Usage & Examples

http://<owas.domain>/we/wordeditorframe.aspx?WOPISrc=http%3a%2f%2flocalhost%3a8080%2fwopi%2ffiles%2fword.docx&access_token=<token>&ui=zh-CN
http://<owas.domain>/p/PowerPointFrame.aspx?WOPISrc=http%3a%2f%2flocalhost%3a8080%2fwopi%2ffiles%2fppt.pptx&access_token=<token>&ui=zh-CN
http://<owas.domain>/x/_layouts/xlviewerinternal.aspx?WOPISrc=http%3a%2f%2flocalhost%3a8080%2fwopi%2ffiles%2fBook1.xlsx&access_token=<token>&ui=zh-CN