namespace Sveltish
open System

type IStore<'T> = interface
    inherit IObservable<'T>
    abstract Update: f:('T -> 'T) -> unit
    abstract Get: 'T
    //abstract Id: int
    end

type Store<'T> = IStore<'T>