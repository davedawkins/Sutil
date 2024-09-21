var path = require("path");

module.exports = {
    entry: "./tests/AllTests.js",
    output: {
        path: path.join(__dirname, "./tests/public"),
        filename: "test-bundle.js",
    },
    devServer: {
        contentBase: "./tests/public",
        port: 8080,
    },
    // module: {
    //     rules: [{
    //         test: /\.fs(x|proj)?$/,
    //         use: "fable-loader"
    //     }]
    // }
}