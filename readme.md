SharpMarkdown
=====
本套件用以剖析Markdown文件結構，簡易的將Markdown文件轉換為物件。

### [Nuget](https://www.nuget.org/packages/SharpMarkdown/1.0.0)
```
PM> Install-Package SharpMarkdown
```

### 快速上手
```csharp
using SharpMarkdown;
...(something)...
//轉換為Markdown內容物件
var mdContent = Markdown.Parse(text);
//進行章節剖析
var mdSection = mdContent.ToSection();

//讀取作為標準的章節結構
var stDoc = Markdown.Parse(standardText).ToSection();

//基礎結構檢驗(標準有的都必須有)
bool isMatchBase = mdSections.IsMatch(stDoc);

//基礎結構檢驗加上順序檢驗(章節順序需要跟標準一樣)
bool isMatchOrder = mdSections.IsMatch(stDoc, Section.MatchModes.Order);

//章節的順序、結構都要完全一樣(標準有的都要有，沒有的不能有)
bool isMatchFull = mdSections.IsMatch(stDoc, Section.MatchModes.Full);

//在Markdown內容中尋找參考標籤
var tag1 = mdSection.FindTag("1");
...(something)...
```
> 詳細的使用範例可見TestConsole專案
> 可以透過剖析Markdown章節結構來確定文件是否符合規範，或取得指定章節的文字

### 演示
1.剖析章節結構

![Imgur](http://i.imgur.com/2dxOSaP.png)

2.章節結構剖析結果

![Imgur](http://i.imgur.com/QfbhFx3.png)

3.章節結構檢驗

![Imgur](http://i.imgur.com/fhAeUL3.png)