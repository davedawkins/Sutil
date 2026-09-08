module Main


#if !HEADLESS

HelloWorldTest.init()
DOMTest.init()
BindingTest.init()
AnchorTest.init()
BindApiTest.init()
ObservableTest.init()
StoreTest.init()

BrowserFramework.runAll()

#endif
