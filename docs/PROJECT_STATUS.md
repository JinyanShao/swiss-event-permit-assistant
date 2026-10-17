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

- 8 条官方规则来源及相关资源于 **2026-10-17** 复核；应用元数据和仓库目录已更新同一日期。逐项页面记录与主管机关待确认问题见[来源复核明细](#来源复核明细)。
- Ville 三条旧人数分档 URL 返回 HTTP 410。Police 期限现引用通用条件版本 8（20/30/60 个工作日）；截止日期暂不计算，避免把工作日误算成自然日。Police locale 申请表现已可访问并直达结果链接；Smart Check 仍链接市府总览页。
- 复核还发现，V0.1 未询问活动持续天数，无法提示活动持续至少 5 天时的 14 天公报公告；“每天预计人数”输入已按市府申请表改清楚。材料和 RC 要求若只在 Formulaire B 或特定场景适用，现显示“待确认”。
- 90 天新鲜度政策下，下次复核不晚于 **2027-01-15**。
- 作品详情页仍显示 **2026-08-14**，需要和仓库/应用的 **2026-10-17** 对齐。
- 待确认问题现包括：多日活动每天人数不同如何分档；Durabilité Directive 无食品/饮料活动的适用范围和小型活动餐具豁免；免费食品/饮品及场地经营者现有牌照下 Patente K 的适用方式；Formulaire A/B 的触发标准；材料和 RC 是否对所有规模必需；私人场地公共活动的 Police locale 流程；OCN 对竞赛性质和跨市镇路线的权限边界；工作日截止日期如何处理当地公假。
- 对上述问题的分机关咨询清单与联系方式见[主管机关确认跟进](#主管机关确认跟进)。Police locale 邮件的 Outlook 发送状态尚未核实；Smart Event、Préfecture de la Sarine 和 OCN 的询问尚未确认发送，因此不能把它们记作已联系或已获答复。

来源与新鲜度规则见 [官方来源登记与新鲜度政策](#官方来源登记与新鲜度政策)。

## 演示服务与部署

- 生产 URL：<https://sepa-jinyan.azurewebsites.net>；Azure App Service Free/F1、Linux、Italy North。
- 2026-10-17 在线探测首页、`/healthz`、`/Assessment` 均返回 HTTP 200，约耗时 0.27/0.11/0.22 秒；这是当次响应，不是冷启动基准，也不构成可用性保证。当前部署仍是手动 zip 发布，没有生产版本标识。
- 若继续使用 F1，冷启动应作为已知体验边界：首位访问者可能在空闲后等待应用唤醒。Azure 文档说明 Free/Shared 没有 SLA，且 F1 不支持 Always On；GitHub 保活只能偶尔探活，不能承诺保持实例常驻。
- 保活配置是每小时 6 次（每 10 分钟一次，错开整点）。GitHub API 显示工作流状态为 `active`，默认分支为 `main`。API 在 **2026-10-05 07:25 至 2026-10-17 18:22 UTC** 只显示 62 次运行；按该时段估算约 1,794 个预期触发，记录覆盖约 **3.5%**。62 次全为成功，运行耗时中位数约 51 秒、最长 84 秒；61 个相邻间隔的中位数约 **4 小时 41 分**，最短约 **2 小时 16 分**、最长约 **9 小时 31 分**。因此不是已记录的 ping 失败或长时间占用 runner；大多数计划触发没有成为可见运行。GitHub 官方说明 schedule 事件在 Actions 高负载时可能延迟或丢弃。API 没有逐个未触发事件的原因，无法进一步证明是某一时段的负载、调度丢弃还是其他 GitHub 调度问题；错开整点也没有使该计划可靠。
- GitHub 账户通知设置已核对：Actions 通知为“GitHub 和 Email，仅失败工作流”；此计划的运行 actor 是 `JinyanShao`，与计划编辑者一致。现有 62 次运行全部成功，所以**通知配置已确认开启，但实际失败邮件投递尚无端到端失败样本验证**。工作流原来的 5 分钟 job 上限也短于三次 90 秒请求加三次 20 秒间隔的最坏 5 分 30 秒；现已改为 6 分钟，确保末次重试和失败摘要有机会完成。
- 托管成本评估（Microsoft Retail Prices API，2026-10-17，Italy North，USD 公售价，约按 730 小时/月）：

  | 方案 | 月费估算 | 适合度与限制 |
  | --- | ---: | --- |
  | 当前 App Service F1 | $0 | 无 SLA；空闲后可能冷启动。继续使用时应向演示者说明这一点，保活工作流不作体验保证。 |
  | App Service Linux Basic B1 + Always On，1 个实例 | **$13.14/月**（$0.018/小时）；全年约 $157.68 | 保留现有 App Service 和 zip 发布方式，改动最少；Always On 解决空闲卸载导致的冷启动。单实例仍会遇到平台重启或故障，不能当作冗余部署。若需两个实例作冗余，粗估约 $26.28/月。 |
  | Azure Container Apps Consumption，最小 1 个副本（0.25 vCPU/0.5 GiB） | 空闲基线约 **$7.88/月**；若本订阅的月度免费用量未被其他容器应用消耗，粗估净额约 **$5.72/月** | 可保留一个空闲副本并按较低 idle 费率计费；请求期间按 active 费率，另可能有日志等费用。当前仓库没有 Dockerfile/容器发布路径，迁移工作多于 B1。 |

- **建议：**预算为零时留在 F1，并明确告知冷启动；若作品演示要求稳定快速，优先考虑同一 App Service 升至 B1 并开启 Always On。现有记录中的 Azure for Students 额度若仍是每年 $100，B1 连续运行的标价会超过该额度；当前剩余额度尚未确认。Container Apps 账面可更低，但需增加容器构建/部署路径；这两种付费方案都尚未部署。
- 本机 Azure CLI 当前无法读取 `asp-sepa-free`（返回 `SubscriptionNotFound`），因此本次无法实时确认 Azure 资源 SKU、学生额度余额或实际账单；上面按 [部署说明](deployment.md)记录的 F1/Italy North 和官方零售价估算。
- 这次对话中已截取首页和问卷首步画面；截图只显示在对话里，未保存为仓库图片或视频。

参考：[部署说明](deployment.md)、[GitHub 保活工作流](https://github.com/JinyanShao/swiss-event-permit-assistant/blob/main/.github/workflows/keep-warm.yml)、[保活运行记录](https://github.com/JinyanShao/swiss-event-permit-assistant/actions/workflows/keep-warm.yml)、[GitHub 定时任务限制](https://docs.github.com/en/actions/how-tos/troubleshoot-workflows#scheduled-events)、[GitHub Actions 通知规则](https://docs.github.com/en/actions/concepts/workflows-and-actions/notifications-for-workflow-runs)、[Azure Always On 与空闲卸载](https://learn.microsoft.com/en-us/azure/app-service/configure-common)、[App Service 规格与 Always On 支持](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/azure-subscription-service-limits)、[App Service 成本与 SLA](https://learn.microsoft.com/en-us/azure/app-service/overview-manage-costs)、[Container Apps 计费和免费额度](https://learn.microsoft.com/en-us/azure/container-apps/billing)、[Container Apps 最小规格](https://learn.microsoft.com/en-us/azure/container-apps/containers#vcpus-and-memory-allocation-requirements)、[Azure Retail Prices API 文档](https://learn.microsoft.com/en-us/rest/api/cost-management/retail-prices/azure-retail-prices)、[Italy North App Service 价格数据](https://prices.azure.com/api/retail/prices?%24filter=serviceName%20eq%20%27Azure%20App%20Service%27%20and%20armRegionName%20eq%20%27italynorth%27%20and%20priceType%20eq%20%27Consumption%27)、[Italy North Container Apps 价格数据](https://prices.azure.com/api/retail/prices?%24filter=serviceName%20eq%20%27Azure%20Container%20Apps%27%20and%20armRegionName%20eq%20%27italynorth%27%20and%20priceType%20eq%20%27Consumption%27)、[Azure for Students 额度与续期](https://learn.microsoft.com/en-us/azure/education-hub/navigate-costs)。

## 恢复开发时

- 每次继续前检查 `git status --short --branch` 和远端提交，避免把旧的本地状态当成当前状态；来源复核记录和应用元数据的日期以逐项核验记录为准。
- 仓库推送不会自动发布生产站点：目前仍需手动 zip 部署。发布后应单独检查生产健康状态和部署版本。
- 当前 F1 的冷启动是已知体验边界；GitHub Actions schedule 不能可靠保温。若演示需要常驻快速响应，先决定是否接受 B1 的持续月费，再升级并打开 Always On。

## 已知缺口与优先级

### 先处理：可信度与运行可靠性

1. 按[主管机关确认跟进](#主管机关确认跟进)先核实 Police locale 邮件是否已发送，再向 Ville de Fribourg（Smart Event）、Préfecture de la Sarine 和 OCN 发送问题；收到书面答复后逐项决定是否调整规则或继续显示“待确认”。
2. 已完成保活调查：GitHub schedule 运行远少于配置且 API 不能提供丢失触发的逐项原因；失败通知偏好已开启，但邮件投递尚无真实失败样本验证。保活仅作探活，F1 冷启动作为已知边界；已比较 B1 和 Container Apps 成本。仍待决定是否为招牌项目付费升至 B1。
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

## 官方来源登记与新鲜度政策

- 最后一次产品规则复核：2026-10-17。
- 公开发布、政策变化或规则变化前重新核对官方来源。
- 距离上次人工核对超过 90 天的来源视为过期，复核前不应沿用为当前规则依据。
- 官方页面存在歧义时，将相关结果标为“待确认”，不推断强制要求。

| Source ID | 主管机关 | 范围 | 官方 URL | 核对日期 | 置信度 |
| --- | --- | --- | --- | --- | --- |
| `SRC-VDF-ENTRY` | Ville de Fribourg | 活动流程、Police locale 与可持续性入口 | https://www.ville-fribourg.ch/organiser-manifestation | 2026-10-17 | High |
| `SRC-VDF-LT200` | Ville de Fribourg | 少于 200 人的 Police locale 期限 | https://www.ville-fribourg.ch/sites/default/files/2026-09/Conditions%20generales%20pour%20les%20manifestations.pdf | 2026-10-17 | High |
| `SRC-VDF-200-1000` | Ville de Fribourg | 200 至 1,000 人的 Police locale 期限 | https://www.ville-fribourg.ch/sites/default/files/2026-09/Conditions%20generales%20pour%20les%20manifestations.pdf | 2026-10-17 | High |
| `SRC-VDF-GT1000` | Ville de Fribourg | 超过 1,000 人的 Police locale 期限 | https://www.ville-fribourg.ch/sites/default/files/2026-09/Conditions%20generales%20pour%20les%20manifestations.pdf | 2026-10-17 | High |
| `SRC-VDF-DURABILITY` | Ville de Fribourg | 可持续性指令、Smart Check 期限、重复使用餐具豁免 | https://www.ville-fribourg.ch/reglements-tarifs/300-13 | 2026-10-17 | High |
| `SRC-FR-PATENTE-K` | Canton de Fribourg | 临时活动 Patente K | https://www.fr.ch/vie-quotidienne/demarches-et-documents/manifestations-temporaires-patente-k | 2026-10-17 | High |
| `SRC-FR-FORM-B` | Canton de Fribourg | Préfecture 表单目录 | https://www.fr.ch/vie-quotidienne/demarches-et-documents/formulaires-des-prefectures | 2026-10-17 | Medium |
| `SRC-OCN-SPORT` | OCN Fribourg | 公共道路体育竞赛 | https://www.ocn.ch/fr/conduire/autorisations/competitions-sportives | 2026-10-17 | High |

相关结果页的表单和指南链接、逐项状态与本次页面观察见下方来源复核明细。资源链接只帮助用户访问主管机关页面，不改变规则触发条件、期限或要求状态。

## 来源复核明细

复核日期：**2026-10-17**。

本次逐项复核项目登记的 8 条官方规则来源，并检查结果页会显示的相关资源链接。项目只覆盖弗里堡市；此记录是官方公开页面核对，不构成主管机关的个案决定或法律意见。

### 核对方式与日期说明

- 核对日期：2026-10-17。对登记来源和资源链接发起 HTTP GET，并查看官方页面正文、页面标注的更新时间及可到达的表单/指南。
- 上一次规则核对日期为 2026-08-23，资源链接核对日期为 2026-09-04。官方站点没有提供完整的历史版本对照，因此下文区分“本次观察到的页面日期/链接状态”和“相对当前规则实现发现的差异”，不声称没有页面编辑。
- Ville 的三条按人数分档旧链接在本次直接请求中均返回 **HTTP 410 Gone**；搜索引擎仍能显示其索引内容，不能据此把旧 URL 视为可用。截止期现引用市府[《Conditions générales pour les manifestations》PDF](https://www.ville-fribourg.ch/sites/default/files/2026-09/Conditions%20generales%20pour%20les%20manifestations.pdf)；申请动作链接到仍返回 HTTP 200 的[Police locale 在线表单](https://www.ville-fribourg.ch/form/po-manifestation)，Smart Check 入口则留在[市府活动总览页](https://www.ville-fribourg.ch/organiser-manifestation)。
- 除上述 Ville 深链接外，本次检查的其他登记页面、指南和表单资源均能以 HTTP 200 或官方重定向打开。此结果只代表本次请求，不是可用性保证。

### 相对上次登记可确认的变化

- 市府人数分档 URL 的可用性已变化：原来作为规则来源和结果资源使用的三条 URL 本次均 HTTP 410；总览页和申请表仍可用。项目已换成当前在线表单和官方通用条件 PDF。
- 上次资源核验记录中，Police locale 表单深链接曾返回错误；本次从现行市府总览页跟入，同一官方表单路由现返回 HTTP 200，因此 Police locale 结果动作已改为直达表单。
- 本次找到的市府通用条件为**版本 8，2026-08-13**。它把 Police locale 的 20/30/60 最低提前期明确写成**工作日**，而应用此前按自然日计算。由于上次 2026-08-23 核验没有留存页面快照，无法断定版本 8 是在那次之后发布还是当时漏检；当前有效文本和应用计算方式存在差异是可确认的。
- 州政府 Patente K 页当前标注最后修改 2026-04-30；Préfecture 表单目录标注最后修改 2025-03-18；B 表单标注自 2023 年 6 月有效。上述页面日期均早于上次规则核验日期，公开页面没有给出更晚编辑日期。
- Formulaire B 的结果资源原本直接指向文件存储路径；现改用州政府当前目录链接到的 `/document/104241` 规范入口。旧 PDF URL 本身本次仍返回 200，但官方目录现提供此文档解析入口。
- OCN 说明页没有显示修改日期；当前在线申请表与 aide-mémoire 均可打开，公开的 2 个月期限与既有规则一致。

### 八条规则来源逐项记录

#### 1. `SRC-VDF-ENTRY` — Ville 活动总览

- [官方页面](https://www.ville-fribourg.ch/organiser-manifestation)，本次 HTTP 200；页面未显示最近修改日期。当前内容称公共地上的活动须向 Police locale 申请；售卖或分发食品和/或饮料的活动须按预计人流提供可持续性概念。该页链接到现在可用的[Police locale 申请表](https://www.ville-fribourg.ch/form/po-manifestation)和 300.13 Directive。
- 市府[Police locale 申请表](https://www.ville-fribourg.ch/form/po-manifestation)本次 HTTP 200，要求填写“每天预计人数”，并把场地图、交通/安全概念标为“如有”。
- 当前市府[通用条件第 1 条](https://www.ville-fribourg.ch/sites/default/files/2026-09/Conditions%20generales%20pour%20les%20manifestations.pdf)将适用范围写为需使用市有土地许可的活动，含公共领域及向公众开放的市有私有领域。不能把这类“市有私有领域”与一般私有场地混为一类。
- **待主管机关确认：**Directive 第 1 条第 2 款称原则上任何须许可的公共地活动都须提供概念，但通用条件和网站总览把要求写在提供食品/饮料的情形下。无食品/饮料活动是否仍有 Smart Check/概念要求，需 Ville 明确答复。人数每天变化时应按日均还是最高日人数分档，也未由表单说明。

#### 2. `SRC-VDF-LT200` — 少于 200 人

- 原[分档页面](https://www.ville-fribourg.ch/organiser-manifestation/moins-de-200)本次 HTTP 410。当前可直接核对的替代依据是市府通用条件 PDF **版本 8，日期 2026-08-13**：少于 200 名预计参加者的 Police locale 申请必须在活动前至少 **20 个工作日**提交。Smart Check 自身的 20 天要求另见 300.13 Directive；两种申请的单位不能混为自然日。
- 对照应用规则：门槛仍是 `<200`，但旧实现按 20 个自然日计算。代码现保留 20 个工作日的文字提示，并将具体日期标为“待确认”，避免显示过晚日期；日期计算待补工作日/节假日处理。
- 市府表单已核实“每天预计人数”，问卷标签现已改成“每天预计人数”。
- **待主管机关确认：**多日活动每天人数不同时，分档用日均、最高日还是逐日不同档位；活动开始日、工作日、法定假日如何确定最终截止日期。

#### 3. `SRC-VDF-200-1000` — 200 至 1,000 人

- 原[分档页面](https://www.ville-fribourg.ch/organiser-manifestation/200-a-1000)本次 HTTP 410。通用条件版本 8 对 200–1,000 名预计参加者规定：Police locale 申请至少提前 **30 个工作日**提交。Smart Check Plus 的 30 天要求由 300.13 Directive 另行规定。
- 对照应用规则：`200 <= attendance <= 1000` 的分档边界一致；恰好 1,000 人属于中型档。旧实现按 30 个自然日算 Police 截止日期，现已改为不显示未经工作日计算的具体日期。
- **待主管机关确认：**人数每日变化的统计口径；只免费提供食品而不提供饮料时的 Smart Check 适用方式。

#### 4. `SRC-VDF-GT1000` — 超过 1,000 人

- 原[分档页面](https://www.ville-fribourg.ch/organiser-manifestation/plus-de-1000)本次 HTTP 410。通用条件版本 8 对超过 1,000 名预计参加者规定：Police locale 申请至少提前 **60 个工作日**提交。Smart Event Plus 的 60 天要求由 300.13 Directive 另行规定。
- 对照应用规则：`>1000` 的边界一致。旧实现按自然日算 Police 截止日期，现改为提示工作日最短提前量、不伪造日期。
- 通用条件还规定活动持续 **5 天或以上**时，申请需在官方公报公告 14 天并允许异议；V0.1 只问一个活动日期，没有询问持续天数，尚无法提示这项已确认的程序规则。
- **待主管机关确认：**人数每日变化时的统计口径；超过 1,000 人但不涉及食品/饮料时的可持续性概念适用范围；Formulaire B 与市府安全/交通概念之间的触发边界。

#### 5. `SRC-VDF-DURABILITY` — Directive 300.13、可持续性与重复使用餐具

- [现行官方 Directive](https://www.ville-fribourg.ch/reglements-tarifs/300-13)标明于 2024-12-16 通过、2025-01-01 生效；本次 HTTP 200。第 3–5 条列出 `<200` 的 Smart Check/20 天、`200–1,000` 的 Smart Check Plus/30 天、`>1,000` 的 Smart Event Plus/60 天；这些条款写“jours”，而 Police locale 通用条件明确写“jours ouvrables”。第 6 条规定可重复餐具，并称“影响较小的活动”可以获豁免。免费提供餐具需按时填写 Smart Check Plus，且受库存限制。
- 当前规则代码的三档表单与期限一致。Directive 没有在该条款中列明“小型活动豁免”的具体条件；旧 2012 版已经被新 Directive 明确废止，不应把旧版条件当作现行标准。
- **待主管机关确认：**(1) 第 1 条第 1 款限定“涉及食品和饮料”的活动与第 1 条第 2 款“原则上所有须许可活动”之间的适用边界；(2) 仅供应免费食品或免费饮料是否触发 Smart Check；(3) 哪些 `<200` 活动可获餐具豁免，以及需不需要事先批准。应用目前对餐具豁免提示核实，不自动承诺豁免。

#### 6. `SRC-FR-PATENTE-K` — 临时活动 Patente K

- [州政府官方服务页](https://www.fr.ch/vie-quotidienne/demarches-et-documents/manifestations-temporaires-patente-k)本次 HTTP 200，页脚标注最后修改 **2026-04-30**。页面明确：临时活动若计划销售饮料和/或食品，须申请 Patente K；至少提前 60 天通过 egov.fr.ch 提交，由活动所在地区 Prefecture 签发。私人土地需附业主签字授权；时段、延时及按活动性质而定的补充材料也须说明。
- 本次未观察到晚于既有 2026-04-30 页面日期的修订标记；对照项目规则，销售食品/饮料和 60 天期限吻合，私人场地的业主授权有明确依据。
- **待 Prefecture 确认：**免费发放食品、非酒精饮料或免费酒精饮料是否需要 Patente K；由餐厅/场地方而不是活动组织者销售时，场地方既有经营许可是否覆盖活动，以及申请人应是谁。现有页面明确的是“销售”，不能据此把免费供应自动判为必须或豁免。

#### 7. `SRC-FR-FORM-B` — Préfecture 表单目录与 Formulaire B

- [Préfecture 表单目录](https://www.fr.ch/vie-quotidienne/demarches-et-documents/formulaires-des-prefectures)本次 HTTP 200，标注最后修改 **2025-03-18**。目录列 Formulaire A 和“Formulaire complémentaire B (manifestation d’importance)”；当前 B PDF 标注自 **2023 年 6 月**有效，并明确要求 Formulaire A 也填写。现行目录的表单链接解析到[规范 Formulaire B PDF](https://www.fr.ch/document/104241)，本次 HTTP 200。
- 表单包含场地位置图、容量、临时设施、节目、交通/停车、消防、安全、医疗、食品、噪声、垃圾和 RC 等分项；B 表单适用时要求附对应材料。官方公开材料没有给出自动触发 Formulaire B 的单一人数阈值。项目继续将它列为“待确认”，而不是所有大活动的确定强制项。
- **待 Prefecture/Ville 确认：**什么情况构成“manifestation d’importance”，Formulaire B 何时必须提交；在线 Patente K 流程中 Form A/B 是上传件、替代件还是仅适用于线下/特定案；临时设施的场地图和描述是否对所有规模都必交。当前 Police locale 表单把场地图、交通/安全概念标为“如有”，而 B 表单只在其适用时要求附图，因此 V0.1 已将这类材料从“必需”降为“待确认”。

#### 8. `SRC-OCN-SPORT` — OCN 公共道路体育竞赛

- [OCN 官方说明](https://www.ocn.ch/fr/conduire/autorisations/competitions-sportives)本次 HTTP 200，页面未标注修改日期。当前说明要求公共道路上的竞赛性体育活动申请许可，列举赛车、跑步、道路自行车和 MTB，申请至少提前 2 个月。
- [当前申请表](https://www.ocn.ch/sites/default/files/2025-10/2025_Demande_autorisation_manifestation_sportive.pdf)本次 HTTP 200，要求路线和时间表、安全/急救方案、至少 CHF 5,000,000 RC、沿途市镇同意；森林路线还需 SFN 预审。[OCN aide-mémoire](https://www.ocn.ch/sites/default/files/2024-08/Aidememoire_manifestations_sportives_20240819.pdf)本次 HTTP 200，发布日期文件名为 2024-08-19，解释流程与附件。
- 对照应用规则：公共道路竞赛触发 OCN、2 个月期限与 OCN 页一致。申请表同时显示 OCN 许可之外还有市镇同意等材料；代码当前的 OCN 动作不会阻止市镇许可提示，但结果应清楚说明审批并行关系。
- **待 OCN/市镇确认：**没有排名的慈善跑/公众徒步是否属于“竞赛性”体育活动；路线跨多个市镇时各市镇许可、OCN 许可与林业预审的先后和责任边界。

### 相关结果资源链接核验

| 资源 | 本次结果 | 处理 |
| --- | --- | --- |
| Ville 活动总览与 Smart Check 入口 | HTTP 200 | Smart Check 相关资源链接；页面还可到达 Police locale 表单。 |
| Ville Police locale 在线申请表 | HTTP 200 | 本次确认恢复可用；应用的 Police locale 动作改为直达此表单。 |
| Ville 《Conditions générales》版本 8（2026-08-13） | HTTP 200 | 当前 Police locale 分档工作日规则来源；应用按工作日提示最低提前量，但不显示错误的自然日日期。 |
| Patente K 服务页 | HTTP 200 | 保留。 |
| Patente K 在线申请指南 `/document/530631` | HTTP 200，官方重定向到 PDF | 保留稳定文档入口。 |
| Formulaire B | HTTP 200 | 改用当前目录解析的规范链接 `/document/104241`。 |
| OCN 竞赛申请表 PDF | HTTP 200 | 保留。 |
| OCN 竞赛 aide-mémoire PDF | HTTP 200 | 保留。 |

其他仍需澄清的项目：当前公开页面没有确认对所有活动一律强制 RC；私人场地向公众开放与“市有私有领域供公众使用”的许可范围不同；Formulaire B / A 的触发条件；OCN 所称竞赛活动的边界。V0.1 对 RC、临时设施图纸、交通/路线文件已改为“待确认”，不再一概输出必需。

规则和链接的本地核对日期现已更新为 2026-10-17；下次不晚于 2027-01-15。公开页面无完整历史版本对照处，应用继续保留未获确认的解释。

## 主管机关确认跟进

跟进记录起始日期：**2026-10-17**。

本文件跟踪来源复核报告中需要主管机关解释的事项。项目只覆盖弗里堡市；询问的是公开的一般流程，不是某场活动的许可申请。收到书面答复后，逐项记录原文、日期、适用范围和规则处理决定；未答复或答复不明确时继续显示“待确认”。

### 当前状态

- 2026-10-17 再次直接请求 Ville 三条旧人数分档页面：仍为 HTTP 410；活动总览页和 Police locale 在线表单为 HTTP 200。
- 现行活动总览页明确写：市有公共领域上，涉及销售或分发食品和/或饮料的活动须按活动期间平均人数提交可持续性概念；它还给出了 Smart Event 联系方式。
- 同一 Ville 300.13 Directive 第 1 条第 1 款将适用对象限定为“涉及食品和饮料”的公共领域活动，第 1 条第 2 款又说一般而言所有须许可的公共领域活动都须提交概念。具体边界仍请 Ville 解释。
- Police locale 现行在线表单字段为“每天预计人数”；场地图、安保/交通概念标注“如有”。这些表单提示没有单独说明在何种案情下会转为必交。
- **Police locale 邮件状态不确定：**Outlook 中已填写邮件，应用先显示草稿已保存；点击“发送”后 UI 操作超时，未能确认是否进入已发送邮件。重试前先检查 Outlook「已发送」和「草稿」，以免重复发送。
- Smart Event、Préfecture de la Sarine、OCN 的询问尚未确认发送。Gmail 连接要求重新认证，Outlook 后续界面也无法读取；没有把任何主管机关答复登记为已收到。

### 官方联系渠道

| 主管机关 | 联系方式 | 官方依据 |
| --- | --- | --- |
| Ville de Fribourg — Police locale / mobilité | `police@ville-fr.ch`；电话 +41 26 351 74 04 | [市府官方活动申请表（2019 PDF）](https://www.ville-fribourg.ch/sites/default/files/inline-files/PO_Autorisation_spectacle_concert_et_autres11_01_2019.pdf)列出邮箱和电话；当前[在线表单](https://www.ville-fribourg.ch/form/po-manifestation)仍直达 Police locale。 |
| Smart Event | `info@smartevent.info`；电话 +41 26 322 40 00 | 市府[组织活动页面](https://www.ville-fribourg.ch/organiser-manifestation)明确提供。 |
| Préfecture de la Sarine | `prefecturesarine@fr.ch`；电话 +41 26 305 22 20 | 州政府[官方联系页面](https://www.fr.ch/diaf/prsa/contact)。 |
| OCN Fribourg | `office@ocn.ch`；电话 +41 26 484 55 00 | OCN[体育竞赛申请资料](https://www.ocn.ch/sites/default/files/2025-10/2025_Demande_autorisation_manifestation_sportive.pdf)列出联系方式。 |

### 按主管机关确认的问题

#### Ville — Police locale / mobilité

1. **多日活动人数口径：**市府总览提到活动期间“平均人数”。Police locale 的 `<200 / 200–1,000 / >1,000` 档位，对每天人数不同的活动是使用日均、最高日，还是其他计算方式？
2. **工作日截止日期：**20/30/60 个工作日应如何倒算？哪些弗里堡公休日不计入，活动日是否计入？是否有正式计算例子或规则？
3. **材料和 RC：**在线表单中的场地图、安保/交通概念标注“如有”。哪些规模或活动类型必须提交？RC 是普遍要求，还是由风险、地点或许可条件逐案要求？
4. **私人场地：**向公众开放的一般私人地块是否需要 Police locale 许可？这与市有但属私法性质、向公众开放的场地是否不同？

#### Ville — Smart Event / durabilité

1. **适用对象：**无食品和饮料的公共领域活动是否仍须提交 Smart Check/可持续性概念？请解释 300.13 第 1 条第 1、2 款与市府活动总览不同表述之间的关系。
2. **供应方式：**免费分发食品、只供应食品、只供应饮料，分别是否触发 Smart 表单和可重复餐具要求？
3. **小型活动豁免：**第 6 条允许少于 200 人的活动免用可重复餐具；适用标准、申请方式、是否须事先批准是什么？外带摊位的即弃餐具例外与该豁免如何区分？
4. **人数平均值：**当每天预计人数不同，“活动期间平均人数”如何计算并对应档位？

#### Préfecture de la Sarine

1. **Patente K：**州政府页面写“销售”食品和/或饮料时申请。免费提供食品、非酒精饮品、免费酒精饮品是否需要 Patente K？
2. **场地方经营者：**若餐厅、酒吧或场地方由持有现行经营许可的经营者销售，是否覆盖该活动？应由活动组织者还是场地方申请？
3. **Formulaire A/B：**“manifestation d’importance”的判断标准和 Formulaire B 的触发条件是什么？在线 Patente K 流程是否仍要求 Form A/B，或二者仅适用于特定活动/流程？
4. **附件与 RC：**Formulaire B 中的设施、场地图、交通/安全和 RC 材料，分别在什么情况下必须提交？

#### OCN Fribourg

1. **竞赛性质：**没有名次或计时的慈善跑、公众徒步，是否属于须许可的“manifestation sportive à caractère de compétition”？判断因素是什么？
2. **跨市镇路线：**路线跨越多个市镇时，OCN 申请、市镇同意和（如适用）森林路线 SFN 预审由谁协调？建议的申请顺序是什么？各市镇需要分别给出同意吗？

### 收到回复后的记录模板

每封回复分别追加一条记录，不把口头转述写成正式规则：

| 日期 | 主管机关/联系人 | 对应问题 | 答复原文或准确摘要 | 是否覆盖官方来源未说明的边界 | 项目处理 | 证据位置 |
| --- | --- | --- | --- | --- | --- | --- |

项目处理限定为：明确适用则修正规则并补测试；有条件或个案裁量则继续“待确认”并显示条件；主管机关表示不属其权限时记录转介对象；未回复不代表默认许可或豁免。

## 下次恢复时的起步清单

- [ ] 先检查 `git status --short --branch` 和远端提交，再从当前工作区继续。
- [x] 2026-10-17：已将远端保活工作流快进合并到 `main` 并推送。
- [ ] 重新检查 `/healthz`、首页和问卷；记录冷启动与正常请求各自耗时。
- [ ] 将作品详情页来源日期更新为 2026-10-17；外部主管机关回复后再记录并决定规则改动。
- [ ] 先修时区与测试边界，再安排下一轮组织者测试。
