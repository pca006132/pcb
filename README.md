# pcb
##自动补全
###命令格式:
存放在commands.txt里，每行当作一条命令  
参数之间必须以空格分隔(也就是说参数里不能有空格，然而如果有必要以后可能会修改)  
reference和$function会自动当作符合，也就是说say #entityID say 和 say hi bye，后面那个可能会被覆盖  
(能补全hi，但是say hi 的时候可能会补全了say)  

####参数格式
* #reference
* <regex pattern>
* {option1|option2...}
* $function(parameters)
* [optional_prefix]things
* text'comment

前四个格式不能嵌套使用，以下为合法格式:
```
[前缀]1-4的任何一个格式'注释
```

####预先定义regex:
* number: 数字，包括小数
* int: 整数
* coor: 数字，不过前面可以有~符号。也可以只有~符号

####支援function:
* scbObj: 返回所有记分板目标
* trigger: 返回所有trigger目标
* team: 返回所有队伍名称
* tag: 返回所有记分板tag
* sound: 自动补全sounds.json的名称(拿取当前位置的字串进行配对)  
* //以上目前都没有用，有这function然而都是没有资料的
* dot(str1, str2): 自动补全dot文件里的字串(xxx.xxx.xxx这类型资料的时候使用) 详细在下方dot部分讲解
* selector: 自动补全选择器参数(拿取当前位置的字串进行配对)

####例子
···
scoreboard objectives add <\w+>'记分板名称 $dot(stat)'判据 <\w+>'记分板显示名称
scoreboard objectives remove $$scbObj'记分板名称
scoreboard objectives setdisplay #displaySlot'显示位置 $scbObj'记分板名称
scoreboard objectives list
scoreboard players {set|add|remove|reset} $selector'实体或假名 $scbObj'记分板名称 <-?\d+>'分数
···

###dot格式:
存放在dot.json里，使用json格式  
格式为"类别":["xxx.xxx.xxx","xxx.xxx.xxx.xxx"]如此类推  
注意，每个选项里面最后部分可以使用#reference，比如stat.#entityID  
类别: $dot(类别,类别)，这样补全的时候就只会补全指定类别的选项(很抱歉，没写参数的话还是什么都不能补全，因为不可能没参数的)
####例子
```
{
    "stat": [
        "stat.faQ",
        "stat.wtf",
        "stat.ICantThinkAnymore",
        "dummy"
    ]
}
```
* $dot(stat)，输入为"stat."时，自动补全内容为["faQ","wtf","ICantThinkAnymore"]
* $dot(stat)，输入为""时，自动补全内容为["stat","dummy"]

###references格式
格式和dot格式类似，不过就是"类别":["选项","选项"]如此类推
注意，选项只会当作普通字串处理
