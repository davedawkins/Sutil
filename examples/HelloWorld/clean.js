var fs = require('fs');

function deleteFolderRecursive(path) {
    if (fs.existsSync(path) && fs.lstatSync(path).isDirectory()) {
        fs.readdirSync(path).forEach( (file, index) => {
            cleanExisting(path + "/" + file);
        });

        console.log(`Deleting directory "${path}"...`);
        fs.rmdirSync(path);
    }
};

function cleanExisting (path)
{
    if (fs.lstatSync(path).isDirectory()) { // recurse
        deleteFolderRecursive(path);
    }
    else {
        console.log (`Deleting file "${path}"`)
        fs.unlinkSync(path);
    }
}

function clean (path)
{
    if (fs.existsSync(path)) cleanExisting(path);
}

console.log("Cleaning...");

const unclean = [
    "./bin",
    "./obj",
    "./.ionide",
    "./node_modules",
    "./.fable",
    "./package-lock.json",
    "./HelloWorld.fs.js",
    "./public/bundle.js"
]

unclean.forEach( (file,index) => clean(file) )

console.log("Clean completed");