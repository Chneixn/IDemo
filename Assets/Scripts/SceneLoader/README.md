# Scene Loader 模块

模块基于Github开源工程[Eflatun. SceneReference](https://github.com/starikcetin/Eflatun.SceneReference)开发

在使用前需要在Package Manager中安装Eflatun. SceneReference

1. Window -> Package Manager -> Add package from git URL...
2. 输入git地址：git+https://github.com/starikcetin/Eflatun.SceneReference.git#4.1.1
3. 将4.1.1替换为需要安装的版本号

创建新场景名为 “Bootstrapper”, 添加空物体, 并挂载 Bootstrapper 脚本

在 Build Setting 中将 Bootstrapper 添加到第一个场景

> 该场景作为第一个加载的场景，且不该在任何时候被卸载
