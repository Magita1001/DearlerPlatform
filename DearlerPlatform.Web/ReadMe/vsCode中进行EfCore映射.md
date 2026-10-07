### CodeFirst

在VsCode里，需要在终端里执行命令，和程序包管理控制台的命令对照如下

| 目标操作 | 在 VS 2022 程序包管理控制台 (PMC) | 在 VS Code 普通终端 (.NET CLI) |
|---|---|---|
| **1. 新增迁移记录** | Add-Migration 迁移名称 | dotnet ef migrations add 迁移名称 |
| **2. 应用到数据库** | Update-Database | dotnet ef database update |
| **3. 撤销最近一次迁移** | Remove-Migration | dotnet ef migrations remove |
| **4. 生成 SQL 脚本** | Script-Migration | dotnet ef migrations script |

#### 1.
首先需要安装工具 `dotnet tool install --global dotnet-ef --version 9.*` 注意这不是 `Nuget` 包，而是独立的工具。
安装SqlServer包和Tool包

#### 2.
注册 `DbContext` 和创建 `Model` 文件，这点和 `Vs2022` 里没有区别，但是这种写法注册时必须带上一个空的构造函数，否则 `dotnet ef` 工具会找不到入口
```
public DearlerPlatformDbContext()
{

}
public DearlerPlatformDbContext(DbContextOptions options) : base(options)
{

}
```

### DbFirst
终端中使用以下代码：

`dotnet ef dbcontext scaffold "Server=127.0.0.1;Database=TestDb;uid=sa;pwd=308213;TrustServerCertificate=true" Microsoft.EntityFrameworkCore.SqlServer -p DearlerPlatform.Core -o Entities -c DearlerPlatformDbContext --force`

#### 📋 命令参数拆解（非常关键，部分可省略）：
| 参数 / 字段 | 作用解释 |
|---|---|
| **"Server=127.0.0.1;..."** | **数据库连接字符串**。工具需要靠它去登录并读取你的数据库表结构。 |
| **Microsoft.EntityFrameworkCore.SqlServer** | **指定数据库提供程序**。告诉工具你用的是 SQL Server（如果用 MySQL 则换成对应的驱动名）。 |
| **-p DearlerPlatform.Core** | **目标项目（Project）**。指定把自动生成的代码文件放到哪个项目文件夹下。 |
| **-o Entities** | **输出目录（Output Dir）**。在你的项目下自动创建一个名为 Entities 的文件夹，用来存放生成的实体类（如 User.cs）。 |
| **-c DearlerPlatformDbContext** | **自定义上下文名称（Context）**。指定生成的 DbContext 类名叫什么。如果不写，默认会叫 TestDbCwContext（数据库名+Context）。 |
| **--force (或 -f)** | **强行覆盖**。如果数据库表结构改了，加上这个参数再次运行，会直接覆盖掉旧的代码文件，更新模型。 |
运行成功后，你会发现 DearlerPlatform.Core 项目下多出了一个 Entities 文件夹，里面躺着全自动生成的 DearlerPlatformDbContext.cs 和根据数据库表生成的 C# 实体类。
