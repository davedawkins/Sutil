//
// Usage:
// % node convert.js > CssEngine.Converted.fs
//

const fs = require('fs');

const src = fs.readFileSync('CssEngine.fs.NeedsConvert', 'utf8');

function capFst(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}

let current = "";
let comments = [];
let engines = [];

let lines = src.split("\n");

for (var i in lines) {
    let line = lines[i++];

    // Some people, when confronted with a problem, think "I know, I'll use regular expressions." Now they have two problems.
    // - jzw
    //
    // https://blog.codinghorror.com/regular-expressions-now-you-have-two-problems/
    //
    // I think I got away with it though - Dave

    let m = line.match(    /member _\.([^_]+)_([\w\d]+)(.*) = h.MakeStyle\(\s*"(.*)"\s*,\s*"(.*)"\s*\)\s*$/   );

    if (m) {
        let fsname = m[1]; // Can spread syntax destructure this neater?
        let fsvalue = m[2];
        let args = m[3];
        let cssname = m[4];
        let cssvalue = m[5];

        if (fsname !== current) {
            console.log("");
            console.log(`type Css${capFst(fsname)}Engine<'Style>(h: CssHelper<'Style>) =`);
            current = fsname
            engines.push(fsname);
        }

        for (var j in comments) {
            console.log(`    ///${comments[j]}`);
        }
        comments = [];
        console.log(`    member _.${fsvalue}${args} = h.MakeStyle("${cssname}", "${cssvalue}")`)
        continue;
    }

    let m2 = line.match(/\/\/\/(.*)\s*$/);
    if (m2) {
        comments.push(m2[1].trimEnd());
    }
}

console.log("");
for (var i in engines) {
    let name = engines[i];
    console.log(`    let _${name} = Css${capFst(name)}Engine(h)`);
}

console.log("");
for (var i in engines) {
    let name = engines[i];
    console.log(`    member _.${name} = _${name}`);
}

