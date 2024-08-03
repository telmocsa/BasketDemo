# BasketDemo
This implementation focuses on code organization and applying SOLID concetps to code structure to allow it to grow and evolve. 

By doing so it allows unit testing using mock for depedencies and even "integration" testing to validade the promotion application logic for the stated requirements.

I didn't implement front or concrete repos as those are external compoments of the core domain that can be whatever we want if the core is SOLID :) 

## Running Instructions

Just: 

``git clone https://github.com/telmocsa/BasketDemo.git``

``cd BasketDemo``

``dotnet test --logger "trx;LogFileName=DetailedTestResults.trx"``

