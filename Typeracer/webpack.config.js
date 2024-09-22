const path = require('path');

module.exports = {
    entry: './src/index.js',  // Source file
    output: {
        path: path.resolve(__dirname, 'wwwroot/js'),  // Where to save the generated file
        filename: 'bundle.js',  // Generated file name
    },
    module: {
        rules: [
            {
                test: /\.js$/,  // Treat all .js files
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',  // Using Babel to transpile
                },
            },
            {
                test: /\.css$/,  // Treat all .css files
                use: ['style-loader', 'css-loader'],  // Loaders to use
            },
            // Loaders for sound files
            {
                test: /\.(mp3|wav)$/,
                type: 'asset/resource',
                generator: {
                    filename: '../sounds/[name][ext]',  // Output to 'wwwroot/sounds'
                },
            },
        ],
    },
    mode: 'development',  // Development mode
};
