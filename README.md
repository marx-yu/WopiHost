### Introduction

This repository contains a Wopi Host demo.
work with Office online 2016(preview). 
Support DOCX Editing, and also PPTX,XLSX.
Welcome any contribution, and discussion of supporting coauthoring

### Requirements

Requires Office online 2016(preview) server. 
*No need Microsoft.CobaltCore.dll assembly.*

### Known Issues

Doesn't support coauthoring.

### Usage & Examples

http://[owas.domain]/we/WordEditorFrame.aspx?WOPISrc=http%3a%2f%2flocalhost%3a8080%2fwopi%2ffiles%2fword.docx&access_token=[token]&ui=zh-CN 

http://[owas.domain]/p/PowerPointFrame.aspx?PowerPointView=ReadingView&WOPISrc=http%3a%2f%2flocalhost%3a8080%2fwopi%2ffiles%2fppt.pptx&access_token=[token]&ui=zh-CN 

http://[owas.domain]/x/_layouts/xlviewerinternal.aspx?WOPISrc=http%3a%2f%2flocalhost%3a8080%2fwopi%2ffiles%2fBook1.xlsx&access_token=[token]&ui=zh-CN
