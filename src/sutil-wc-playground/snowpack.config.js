/** @type {import("snowpack").SnowpackUserConfig } */
module.exports = {
    mount: {
        public: { url: '/', static: true },
        src: { url: '/dist' },
        "../Sutil": { url: '/Sutil', static: true }
    },
    plugins: ['@snowpack/plugin-dotenv'],
    routes: [],
    optimize: {
        /* Example: Bundle your final build: */
        bundle: true,
        splitting: true,
        treeshake: true,
        manifest: true,
        target: 'es2017',
        minify: true
    },
    packageOptions: {
        /* ... */
    },
    devOptions: {
        /* ... */
    },
    buildOptions: {
        /* ... */
        clean: true,
        out: "dist"
    },
    exclude: [
        "**/*.{fs,fsproj}",
        "**/bin/**",
        "**/obj/**"
    ],
    /* ... */
};