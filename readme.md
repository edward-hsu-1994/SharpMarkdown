SharpMarkdown
=====
This library used to analyze the structure of the Markdown file.
Simple converts document string into a Markdown object or structure.

本套件用以分析Markdown文件結構，簡易的轉換檔案字串為Markdown物件與結構。

### [Nuget](https://www.nuget.org/packages/SharpMarkdown/1.0.0)
```
PM> Install-Package SharpMarkdown
```

### Get Started(快速上手)
```csharp
using SharpMarkdown;
...(something)...
//Convert string to Markdown object(轉換為Markdown物件)
var mdContent = Markdown.Parse(text);
//Analysis section structure(進行章節剖析)
var mdSection = mdContent.ToSection();

//Loading document section structure sample(讀取文件章節結構樣本)
var stDoc = Markdown.Parse(standardText).ToSection();

//Basic test(基本檢驗)
bool isMatchBase = mdSections.IsMatch(stDoc);

//Basic test and Section order test(章節順序檢驗)
bool isMatchOrder = mdSections.IsMatch(stDoc, Section.MatchModes.Order);

//complete structure
bool isMatchFull = mdSections.IsMatch(stDoc, Section.MatchModes.Full);

//find reference tag
var tag1 = mdSection.FindTag("1");
...(something)...
```

### Demo(演示)
1.Analyze the structure of the Markdown file(剖析章節結構)

![Imgur](http://i.imgur.com/2dxOSaP.png)

2.Analyze result(分析結果)

![Imgur](http://i.imgur.com/QfbhFx3.png)

3.Markdown section structure test(章節結構測試)

![Imgur](http://i.imgur.com/fhAeUL3.png)
