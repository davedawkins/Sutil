module Main


#if !HEADLESS

HelloWorldTest.init()
DOMTest.init()
BindingTest.init()
ObservableTest.init()
StoreTest.init()

BrowserFramework.runAll()

#endif
