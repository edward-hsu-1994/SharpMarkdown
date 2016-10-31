SharpMarkdown
=====
本套件用以剖析Markdown文件結構，簡易的將Markdown文件轉換為物件。

### 快速上手
```csharp
using SharpMarkdown;
...(something)...
//轉換為Markdown內容物件
var mdContent = Markdown.Parse(text);
//進行章節剖析
var mdSection = mdContent.ToSection();
//在Markdown內容中尋找參考標籤
var tag1 = mdSection.FindTag("1");
...(something)...
```
> 詳細的使用範例可見TestConsole專案

### Demo
1.剖析章節結構![Imgur](http://i.imgur.com/2dxOSaP.png)
2.章節結構剖析結果![Imgur](http://i.imgur.com/QfbhFx3.png)
