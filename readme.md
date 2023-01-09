An industrial revolution themed game commited to [Historically Accurate Game Jam 6](https://itch.io/jam/historically-accurate-game-jam-6).

<img src="https://img.itch.zone/aW1hZ2UvMTg2ODk4Ni8xMDk3OTg2MS5wbmc=/original/rY%2Bsv8.png">
<img src="https://img.itch.zone/aW1hZ2UvMTg2ODk4Ni8xMDk3OTg2NC5wbmc=/original/CKo0GJ.png">

## Models

Given equations:

$$
\begin{align*}
SupplyCotton &= CottonScale \times Max(CottonPrice - CottonPriceFarmThres, 0)^{CottonPower} \\
SupplyWorkhour &= WorkhourScale \times Max(WorkhourPrice - WorkhourPriceWorkerThres, 0)^{WorkhourPower} \\
DemandCotton &= DemandCottonWorkshop + DemandCottonFactory \\
DemandWorkhour &= DemandWorkhourWorkshop + DemandWorkhourFactory \\
DemandCottonWorkshop &= CottonWorkshopCost \times WorkshopTextileSupply \\
DemandCottonFactory &= CottonFactoryCost \times FactoryTextileSupply \\
DemandWorkhourWorkshop &= WorkhourWorkshopCost \times WorkshopTextileSupply \\
DenabdWorkhourFactory &= WorkhourFactoryCost \times FactoryTextileSupply \\
WorkshopTextileSupply &= WorkshopTexileScale \times Max(TextilePrice - WorkhourWorkshopCost \times WorkhourPrice - CottonWorkshopCost \times CottonPrice - TextilePriceWorkshopThres, 0)^{TextileWorkshopPower} \\
FactoryTextileSupply &= FactoryTextileScale \times Max(TextilePrice  - WorkhourFactoryCost \times WorkhourPrice - CottonFactoryCost \times CottonPrice - TextilePriceFactoryThres, 0)^{TextileFactoryPower} \\
ConsumerDemand &= WorkshopTextileSupply + FactoryTextileSupply \\
ConsumerDemand &= TextileDemandCoef / TextilePrice
\end{align*}
$$

CottonPrice, WorkhourPrice, and TextilePrice are "solved" in a iterative manner. For example, $CottonPrice_t$ is solved by fixing $WorkhourPrice_{t-1}, TextilePrice_{t-1}$ and find the intersection point of 1d demand curve and supply curve. The root finding leverage the root finding algorithm of `MathNet.Numberical`.

A early exploration notebook:

https://gist.github.com/yiyuezhuo/5edbbb682d962073560ba1ec53cb2bf4
