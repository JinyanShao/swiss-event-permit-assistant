# Swiss Event Permit Assistant：项目状态与恢复笔记

最后整理：2026-10-17。用于恢复项目上下文；线上状态和官方来源会变化，继续开发前请重新核实。

## 一分钟状态

- 项目已完成并发布 V0.1.5，是一个可访问的真实产品，不是待实现的原型。
- 当前产品范围：仅覆盖弗里堡市的临时活动准备；法语界面；生成可能涉及的手续、材料、期限、待确认事项和官方来源。它不是官方服务或法律意见，不提交申请，也不保存服务器端活动档案。
- 目前最适合的阶段是补强运行可靠性、来源可信度和用户验证，再根据反馈决定 V0.2；不需要先扩展到整个瑞士。
- 最近一次线上检查（2026-10-17）：首页和 `/healthz` 均 HTTP 200，健康检查连续 3 次返回 `Healthy`；Chrome 能打开首页和问卷第 1/7 步。先前的超时仍可能是间歇性故障，本次快照不代表 SLA。

## 最近进展

- 2026-08-16：发布 V0.1.0，完成核心问卷、规则评估和结果页。
- 2026-08-23：复核产品规则来源；完成第一轮 1 位匿名法语测试者的反馈调整。
- 2026-09-03：协会支持中间人反馈了“私人场地但向公众开放”的理解问题；产品随后分开询问场地属性和活动开放范围。
- 2026-09-04：检查并替换失效的官方深链接，改用稳定的官方信息入口。
- 2026-09-14：打磨问卷导航和 sitemap。
- 2026-09-27：生产环境迁移到 Azure for Students；移除 Plausible 分析和跟踪代码。
- 2026-09-29：发布 V0.1.5，包含校验、草稿恢复、结果资源链接和文档整理。
- 2026-10-05：远端 `main` 增加 GitHub Actions 定时 ping，尝试缓解 Azure 免费层冷启动。
- 2026-10-17：重新核对 8 条官方来源和相关资源；发现三条旧人数分档链接 HTTP 410，并找到市府《Conditions générales》2026-08-13 版本 8，明确 Police locale 期限是 20/30/60 个工作日。应用更新来源和资源链接，停止按自然日显示 Police locale 截止日期，并把依据不足的材料要求改为“待确认”。

详细记录见 [CHANGELOG.md](../CHANGELOG.md)、[第一轮用户测试](user-testing/round-1.md) 和 [第二轮外部反馈](user-testing/round-2.md)。第二轮是协会支持中间人的反馈与转介信号，不是合作、官方背书、正式采用或协会用户量证明。

## 产品与技术概况

- 七步流程：范围、活动、地点、饮食、音乐与设施、交通、最终核对。
- 规则在独立的 Domain 项目中评估；结果条目带稳定 ID 和来源 ID。遇到来源无法确认的情形时，使用“待确认”，而不是把不确定性伪装成强制要求。
- 期限按活动日期计算，使用 `TimeProvider`；规则测试可以固定时钟。
- 技术栈为 .NET 10、ASP.NET Core Razor Pages、C# 和 xUnit。没有数据库、账号、文件上传或运行时官方来源抓取；问卷草稿仅保存在浏览器本地。
- 仓库有 65 个 xUnit 测试。本次来源复核和规则调整后，`dotnet test SwissEventPermitAssistant.slnx --no-restore` 本地通过：65 passed、0 failed。
- 项目有两轮外部反馈记录，但已移除分析追踪，因此目前没有可支撑采用量或完成率的使用数据。

参考：[项目范围](../README.md)、[架构决策记录](decisions.md)、[威胁模型](security/threat-model.md)。

## 来源状态

- 8 条官方规则来源及相关资源于 **2026-10-17** 复核；应用元数据和仓库目录已更新同一日期。逐项页面记录与主管机关待确认问题见[来源复核报告](sources/review-2026-10-17.md)。
- Ville 三条旧人数分档 URL 返回 HTTP 410。Police 期限现引用通用条件版本 8（20/30/60 个工作日）；截止日期暂不计算，避免把工作日误算成自然日。Police locale 申请表现已可访问并直达结果链接；Smart Check 仍链接市府总览页。
- 复核还发现，V0.1 未询问活动持续天数，无法提示活动持续至少 5 天时的 14 天公报公告；“每天预计人数”输入已按市府申请表改清楚。材料和 RC 要求若只在 Formulaire B 或特定场景适用，现显示“待确认”。
- 90 天新鲜度政策下，下次复核不晚于 **2027-01-15**。
- 作品详情页仍显示 **2026-08-14**，需要和仓库/应用的 **2026-10-17** 对齐。
- 待确认问题现包括：多日活动每天人数不同如何分档；Durabilité Directive 无食品/饮料活动的适用范围和小型活动餐具豁免；免费食品/饮品及场地经营者现有牌照下 Patente K 的适用方式；Formulaire A/B 的触发标准；材料和 RC 是否对所有规模必需；私人场地公共活动的 Police locale 流程；OCN 对竞赛性质和跨市镇路线的权限边界；工作日截止日期如何处理当地公假。
- 对上述问题的分机关咨询清单与联系方式见[主管机关确认跟进](sources/authority-clarifications-2026-10-17.md)。Police locale 邮件的 Outlook 发送状态尚未核实；Smart Event、Préfecture de la Sarine 和 OCN 的询问尚未确认发送，因此不能把它们记作已联系或已获答复。

来源与新鲜度规则见 [docs/sources/README.md](sources/README.md)。

## 演示服务与部署

- 生产 URL：<https://sepa-jinyan.azurewebsites.net>；Azure App Service Free/F1、Linux、Italy North。
- 本次在线检查成功，但该免费层有冷启动记录且没有 SLA；部署仍为手动 zip 部署，CI 不会自动发布，也没有生产版本标识。
- 远端保活工作流配置目标为每 10 分钟运行一次；GitHub Actions API 在 2026-10-05 至 2026-10-17 期间只显示 62 次成功运行，约每天 5 次，远低于配置频率。运行成功不代表实际频率足以避免冷启动。GitHub 说明计划任务可能延迟或被丢弃。
- 这次对话中已截取首页和问卷首步画面；截图只显示在对话里，未保存为仓库图片或视频。

参考：[部署说明](deployment.md)、[GitHub 保活工作流](https://github.com/JinyanShao/swiss-event-permit-assistant/blob/main/.github/workflows/keep-warm.yml)、[保活运行记录](https://github.com/JinyanShao/swiss-event-permit-assistant/actions/workflows/keep-warm.yml)、[GitHub 定时任务限制](https://docs.github.com/en/actions/reference/workflows-and-actions/events-that-trigger-workflows#schedule)。

## 恢复开发时

- 每次继续前检查 `git status --short --branch` 和远端提交，避免把旧的本地状态当成当前状态；来源复核记录和应用元数据的日期以逐项核验记录为准。
- 仓库推送不会自动发布生产站点：目前仍需手动 zip 部署。发布后应单独检查生产健康状态和部署版本。
- 免费层可能冷启动；保活工作流的实际执行频率曾低于配置目标，重新评估演示可用性时应检查最新 Actions 运行记录。

## 已知缺口与优先级

### 先处理：可信度与运行可靠性

1. 按[主管机关确认跟进](sources/authority-clarifications-2026-10-17.md)先核实 Police locale 邮件是否已发送，再向 Ville de Fribourg（Smart Event）、Préfecture de la Sarine 和 OCN 发送问题；收到书面答复后逐项决定是否调整规则或继续显示“待确认”。
2. 调查保活计划为何实际运行远少于配置频率，并确认失败通知有效。若继续使用免费层，就把冷启动作为已知体验边界；若演示必须稳定快速，再评估支持常驻实例的托管方案及成本。
3. Deadline 状态目前依赖服务器本地时区；决策记录指出 Azure Linux 使用 UTC，在苏黎世午夜附近可能偏一天。应显式按 `Europe/Zurich` 计算，并覆盖日期边界。Police locale 期限还须按工作日处理（含公假规则）；目前只显示最低工作日提前量，没有计算截止日。

### 再补：防回归能力

4. 为人数阈值 `199/200/1000/1001` 和“距截止 14 天”的状态边界加测试。
5. 增加至少一条完整的问卷到结果页自动化路径；现有测试主要是规则单元测试和源码字符串断言，没有浏览器 E2E 或 HTTP 集成测试。
6. 修正结果页对无效 JSON、缺少人数等错误统一显示“缺少日期”的提示；按威胁模型补充请求体大小限制。
7. 为手动生产部署增加可核对的版本标识，降低无法确认线上版本是否与 `main` 一致的风险。

参考：[已记录的限制和测试缺口](decisions.md#adr-003--compute-deadlines-from-the-event-date-with-an-injected-clock)、[安全与部署决策](decisions.md#adr-005--server-rendered-razor-pages-no-database-browser-only-draft-french-first-ui)、[部署决策](decisions.md#adr-007--deploy-to-azure-app-service-free-f1-linux-by-manual-zip-deploy-with-a-healthz-endpoint)。

### 用户验证与作品展示

8. 邀请几位真实活动组织者测试代表性场景：市辖区公共空间、向公众开放的私人场地、免费食品或饮品、道路体育活动、范围外的市镇。记录任务是否完成、哪里困惑、哪些说明促成了主管机关确认；不要把小样本写成采用量或官方验证。
9. 将项目案例页写成可审查的故事：问题与用户、规则如何追溯到官方来源、关键取舍、测试和外部反馈、明确的范围限制；补上结果页截图或短视频。
10. 等新反馈再选一个 V0.2 方向。德语界面可作为候选；跨市镇扩展、PDF 自动填表和账号存储都会扩大来源维护或隐私责任，先不要同时推进。

## 下次恢复时的起步清单

- [ ] 查看 `git status --short`，保留 `README.md` 的来源日期更正。
- [ ] 将本地 `main` 快进到 `origin/main`，纳入 2026-10-05 的保活工作流。
- [ ] 重新检查 `/healthz`、首页和问卷；记录冷启动与正常请求各自耗时。
- [ ] 将作品详情页来源日期更新为 2026-10-17；外部主管机关回复后再记录并决定规则改动。
- [ ] 先修时区与测试边界，再安排下一轮组织者测试。
