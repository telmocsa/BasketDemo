# BasketDemo

## Design Considerations

This follows a domain centric approach defining dependencies that would be injected with concrete instances from the outside. 

It works with and defines abstracted dependencies only implementing business rules. 

As such besides unit testing, I could take advantage of theses abstractions (interfaces mostly) to mock dependencies that allowed to do a kind on integrated test to validate promotions business rules.

I've tried to implement a promotions logic that would allow me to do promotion composition applied in the BuyX Get Y with a discout of Z where I compose two promotions
* Buy X with ammount # get Y 
* Percentage Off ##

I refer in models to an abstract concept of ``Promotion`` and then, in runtime, work with concrete stragegy of the actual applied promotion. 

## Running Instructions

Just: 

``git clone https://github.com/telmocsa/BasketDemo.git``

``cd BasketDemo``

``dotnet test --logger "trx;LogFileName=DetailedTestResults.trx"``

