# EthereumTestTask

This is a result of the test task.
It has three main detectors which correspond to each part of a task:
 - ERC20TokenDetector is responsible for detecting ERC20 transfers (Part A);
 - EthTransferDetector is responsible for detecting ETH transfers (Part B);
 - TransferFromContractDetector is responsible for detecting transfers from a smart contract(Part C);
 
 appsettings.json has:
  - links to eth-clients (It should be provided with both HTTP and web socket addresses);
  - list of addresses which are wanted to be discovered.
  
  There are drawbacks of the application:
  - the app is monitoring only addresses which were loaded in startupTime. If you want to add, remove, or update an address, you need to update appsettings and restart the app.
  - the app doesn't track previous events. It only relies on newly generated information. So if it stopped and started again, it would probably have gaps. 
  - the app writes only logs. It doesn't save any information to db or to any third-party apps.

  It should be mentioned about drawbacks in the implementation of Part C.
  It relies only on events generated by a smart contract. The generator supposes that the event would have somewhere in log.Topics an monitoring address.
  So in the next conditions we would have a false-alarm log:
  - A smart contract logged an event with a monitoring address but did not really transfer money to the monitoring address;
  - if the event look like  event TestEvent(address[] array). In this case, it won't be possible to track it in THIS implementation.
  
  
