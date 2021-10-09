var path = require("path");

module.exports = {
    entry: "./test/HelloWorldTest.fs.js",
    output: {
        path: path.join(__dirname, "./public"),
        filename: "test-bundle.js",
    },
    devServer: {
        contentBase: "./public",
        port: 8080,
    },
    // module: {
    //     rules: [{
    //         test: /\.fs(x|proj)?$/,
    //         use: "fable-loader"
    //     }]
    // }
}
