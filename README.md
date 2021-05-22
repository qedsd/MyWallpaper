# MyWallpaper
简简单单的Windows壁纸管理器，可添加文件夹、图片、必应壁纸、本地聚焦作为壁纸。

## 项目结构

### MyWallpaper
管理器UI，.net 5 + wpf，仅用于配置、开启、关闭壁纸服务，不直接修改壁纸。

### MyWallpaperService
常驻后台的真正起修改壁纸功能的 .net 5 控制台程序，定期读取由MyWallpaper负责修改的配置文件 *config.xml* 以修改壁纸。原本是一个真正的worker service项目，结果启用服务后无法成功调用*SystemParametersInfo* 来修改壁纸，未解，先写个隐藏界面在后台运行的控制台凑合（又不是不能用，内存占用还比worker service少几m呢）

## 使用（从源码生成）

1.生成发布MyWallpaper、MyWallpaperService

2.在MyWallpaper.exe同一路径下创建一个如下bat文件

MyWallpaperServiceSetup.bat
```
@ECHO ON  
f: 
cd /d %~dp0
start  /b  workerservice/MyWallpaperService.exe -f  
```

此bat用于将MyWallpaperService在后台无ui运行。

3.在MyWallpaper.exe同一路径下创建文件夹Img，内含图片文件如下


| 文件名 | 作用 |
| ------- | ------- |
|bg.jpg|管理器UI背景|
|floder.png|管理器添加文件夹后的图标|
|plus.png|管理器添加文件/文件夹操作图标|

使用过程还会产生download、temp.bmp文件分别

4.在MyWallpaper.exe同一路径下创建文件夹workerservice，将MyWallpaperService生成文件放在此文件夹内

5.运行MyWallpaper.exe

## 使用（直接使用）

下载releases压缩包解压即可打开MyWallpaper.exe使用（不确保最新）
