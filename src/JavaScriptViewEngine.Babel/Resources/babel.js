import {transform as bTransform, version as bVersion} from 'babel-standalone';

export function babelTransform(input, babelConfig, filename) {
    babelConfig = {
		...JSON.parse(babelConfig),
        ast: false,
        filename,
    }
    try {
        return bTransform(input, babelConfig).code;
    } catch (ex) {
        throw new Error(ex.message);
    }
}

export function babelTransformSourcemap(input, babelConfig, filename) {
    babelConfig = {
		...JSON.parse(babelConfig),
        ast: false,
        filename,
        sourceMaps: true,
    };
    try {
        var result = babelTransform(input, babelConfig);
        return JSON.stringify({
            bVersion,
            code: result.code,
            sourceMap: result.map
        });
    } catch (ex) {
        throw new Error(ex.message);
    }
}