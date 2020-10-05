# CranBattleDefeatLogViewer

クランバトルでのバトルログを視覚化するツールです。  
ログを元にグラフを出力します。  

![timeline](https://github.com/izmktr/CranBattleDefeatLogViewer/blob/master/image/timeline.png?raw=true)

どのボスが出現しているか、を表す図です。  
青い部分なら1ボスが出現しています。  

![attack](https://github.com/izmktr/CranBattleDefeatLogViewer/blob/master/image/attack.png?raw=true)

凸推移です。  

![attackspeed](https://github.com/izmktr/CranBattleDefeatLogViewer/blob/master/image/attackspeed.png?raw=true)

凸時速です。  
前後30分の差分です。  
10時なら、9時半～10時半の凸の差分となっています。  
(9:00が3凸、9:40が4凸の場合、9:30は3.75凸と計算しています。)  

## ダウンロード

[こちら](https://github.com/izmktr/CranBattleDefeatLogViewer/raw/master/Execute/CranBattleDefeatLogViewer.zip) よりダウンロードしてください。
サンプル用の戦闘データも入っています。

## 使い方

戦闘ログをウィンドウにD&Dしてください。  
用意する戦闘ログは以下のようになります  

    [defeatlog.txt]  
    2020/06/25 06:26:59  
    2020/06/25 06:46:07  
    2020/06/25 07:13:06  
    2020/06/25 07:17:17  
    2020/06/25 07:45:25  

1行目から1ボスを倒した時刻、2ボスを倒した時刻、というのを書きます。  

[ユカリさんbot](https://note.com/izmktr/n/n5de070a975c7)を使っている場合、凸報告で「defeatlog」で生成されます。  

    [attacklog.txt]  
    2020/06/25 06:26:59  
    2020/06/25 06:46:07  
    2020/06/25 07:13:06  
    2020/06/25 07:17:17  
    2020/06/25 07:45:25  

1行目から1凸を終えた時刻、2凸を終えた時刻、というのを書きます。  

[ユカリさんbot](https://note.com/izmktr/n/n5de070a975c7)を使っている場合、凸報告で「attacklog」で生成されます。  

