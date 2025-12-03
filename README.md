## 由于 API 接口变动和项目规划调整，本项目已不再进行维护，请您移步本组织目录下的其他项目

# SwarmCloneUnity
这里是 SwarmClone 的直播画面合成项目。
## 技术实现方式
1. 为本项目接入live2D SDK
2. 利用Unity的UnityWebRequest异步发送请求到我们的AI服务器，并接收响应，根据AI返回的情感分析结果，更新Live2D模型的参数来改变动作和表情。
在没有接收到响应时，采用算法随机进行一些晃动让模型的表现更自然。
3. 后续步骤

## 开发注意事项
1. 使用最新的 unity 2022 LTS 进行本项目的开发
2. 使用简洁明确的语言编写提交消息，并统一在提交消息前加上“【】”概括您提交代码的属性</br>
   例如：【修复】【新增】【优化】【更改】等

## 项目开发规范
参见：https://blog.csdn.net/LegendaryChen/article/details/129142162

## 相关链接
Live2D SDK for Unity官方文档：https://docs.live2d.com/zh-CHS/cubism-sdk-tutorials/getting-started/
