namespace Sutil

///  <exclude />
module Random =
    let shuffleR xs = xs |> Seq.sortBy (fun _ -> int(Interop.random() * 100.0))

    let pickOne xs = xs |> shuffleR |> Seq.head

    let randomInt min max =
        min + int(System.Math.Round(Interop.random() * float(max-min)))

    let randomSign(n:float)=
        n * if randomInt 0 1 = 0 then 1.0 else -1.0


open Random
type private Edit =
    | Nop
    | Create
    | Update
    | Delete
///  <exclude />
type DataSimulation =
    static member Stream (init : unit -> 'T) (f : int -> 'T -> 'T) (delay : int) =
        let mutable dispose : unit -> unit = Unchecked.defaultof<_>
        let mutable tick : int  = 0
        let store = ObservableStore.makeStore init (fun _ -> dispose())
        dispose <- DomHelpers.interval (fun _ -> tick <- tick + 1; Store.modify (fun v -> f tick v) store) delay
        store

    static member CountList (min : int, max : int, delay : int) =
        let n = max - min + 1
        DataSimulation.Stream
            (fun () -> [])
            (fun t current ->
                let i = t % n
                if (i = 0) then [ min ]
                else current @ [ min + i ]
            )
            delay

    static member Records (data : 'T list, update: 'T -> 'T, maxEditsPerTick : int, allowCreateDelete : bool, delay : int) : IStore<'T list> =
        let dataLen = data.Length
        let mutable removed = [ ]

        let chooseEdit (current : 'T list) =
            if (allowCreateDelete) then
                if List.isEmpty current && not (List.isEmpty removed) then
                    Edit.Create
                else if List.isEmpty removed then
                    pickOne [ Edit.Update; Edit.Delete ]
                else
                    pickOne [ Edit.Create; Edit.Update; Edit.Delete ]
            else
                Edit.Update

        let editOne current =
            match chooseEdit current with
            | Edit.Nop -> current
            | Edit.Create ->
                let item = pickOne removed
                removed <- removed |> List.filter (fun r -> r <> item)
                current @ [ item ]
            | Edit.Delete ->
                let item = pickOne current
                removed <- item :: removed
                current |> List.filter (fun r -> r <> item)
            | Edit.Update ->
                let item = randomInt 0 (dataLen-1)
                current |> List.mapi (fun i r -> if i = item then update r else r)

        let edit _ current =
            let n = 1 + int(Interop.random() * float maxEditsPerTick)
            [1..n] |> List.fold (fun current' _ -> editOne current') current

        upcast DataSimulation.Stream (fun () -> data) edit delay

    static member Records (data : 'T list, update: 'T -> 'T, delay : int) : IStore<'T list> =
        DataSimulation.Records(data,update,1,true,delay)

    static member Count (min : int, max : int, delay : int) =
        DataSimulation.Stream
            (fun _ -> min)
            (fun _ n -> if n = max then min else n + 1)
            delay

    static member Random (min : float, max : float, delay : int) =
        let next() = min + (max - min) * Interop.random()
        DataSimulation.Stream next (fun _ _ -> next()) delay

    static member Random (min : int, max : int, delay : int) =
        let next() = min + int(System.Math.Round(float(max - min) * Interop.random()))
        DataSimulation.Stream next (fun _ _ -> next()) delay

    static member Random (max:int) =
        DataSimulation.Random(1,max,1000)

    static member Random () =
        DataSimulation.Stream Interop.random (fun _ _ -> Interop.random()) 1000

///  <exclude />
module SampleData =
    type Todo = {
        Description : string
        Completed : bool
    }
    type Name = {
        Name : string
        Surname : string
        Email : string
        Zip : string
    }
    type Stock = {
        Symbol : string
        Price : float
    }

    let updateStock (r : Stock) =
        { r with Price = r.Price * (1.0 + randomSign(0.01)) }

    let sampleStocks (number:int) =
        let nextSymbol() = [ 'A'..'Z' ] |> shuffleR |> Seq.take 4 |> Seq.fold (fun s c -> sprintf "%s%c" s c) ""
        [1..number]
        |> List.map (fun _ -> { Symbol = nextSymbol(); Price = System.Math.Round(Interop.random() * 100.0,2) })

    let stockFeed numStocks delay = DataSimulation.Records(sampleStocks(numStocks),updateStock,5,false,delay)

    let sampleTodos() = [
        { Description = "Write documentation"; Completed = false }
        { Description = "Create website"; Completed = true }
        { Description = "Check spellling"; Completed = false }
        { Description = "Write tests"; Completed = false }
        { Description = "Create package"; Completed = true }
        { Description = "DevTools for Firefox"; Completed = true }
        { Description = "DevTools for Chrome"; Completed = false }
        { Description = "Implement benchmarks"; Completed = false }
        { Description = "Create templates"; Completed = false }
        { Description = "WebGL version"; Completed = false }
    ]

    let updateTodo (r : Todo) =
        { r with Completed = not r.Completed }

    let todosFeed numTodos delay = DataSimulation.Records(sampleTodos() |> List.take numTodos,updateTodo,1,true,delay)

    let sampleNames = [
        {
            Name = "Alvin"
            Surname = "Melendez"
            Email = "tristique@ligulaNullam.net"
            Zip = "50619"
        }
        {
            Name = "Brooke"
            Surname = "Cline"
            Email = "ullamcorper.nisl.arcu@ligulaAenean.co.uk"
            Zip = "61195"
        }
        {
            Name = "Jameson"
            Surname = "Douglas"
            Email = "Donec.est@mollisneccursus.ca"
            Zip = "494841"
        }
        {
            Name = "Uta"
            Surname = "Wade"
            Email = "dapibus.rutrum.justo@auctornonfeugiat.net"
            Zip = "Z4008"
        }
        {
            Name = "Burke"
            Surname = "Guerra"
            Email = "tristique.senectus.et@Quisque.com"
            Zip = "68-482"
        }
        {
            Name = "Joseph"
            Surname = "Abbott"
            Email = "risus@mollis.ca"
            Zip = "432199"
        }
        {
            Name = "Alma"
            Surname = "Dominguez"
            Email = "augue@nibhPhasellus.ca"
            Zip = "CX8 3LX"
        }
        {
            Name = "Buckminster"
            Surname = "Bauer"
            Email = "enim.Nunc.ut@Integer.co.uk"
            Zip = "R3X 6T2"
        }
        {
            Name = "Belle"
            Surname = "Wilcox"
            Email = "mollis.non.cursus@amet.edu"
            Zip = "60090"
        }
        {
            Name = "Caleb"
            Surname = "Burnett"
            Email = "quam@ornare.com"
            Zip = "56446"
        }
    ]
