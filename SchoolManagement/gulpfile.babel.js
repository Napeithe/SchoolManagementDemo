/// <binding AfterBuild='default' ProjectOpened='watch' />
import gulp from 'gulp';
import sass from 'gulp-sass';
import babel from 'gulp-babel';
import concat from 'gulp-concat';
import uglify from 'gulp-uglify';
import rename from 'gulp-rename';
import cleanCSS from 'gulp-clean-css';
import del from 'del';

const paths = {
    styles: {
        src: './Features/**/*.scss',
        dest: './wwwroot/css'
    },
    scripts: {
        src: "./Features/**/*.js",
        dest: './wwwroot/js'
    }
};

/*
 * For small tasks you can export arrow functions
 */
export const clean = () => del(['assets']);

/*
 * You can also declare named functions and export them as tasks
 */
export function stylesDev() {
    return gulp.src(paths.styles.src)
        .pipe(sass())
        .pipe(concat('site.css'))
        .pipe(gulp.dest(paths.styles.dest));
}


export function scriptsDev() {
    return gulp.src(paths.scripts.src, { sourcemaps: true })
        .pipe(babel())
        .pipe(concat('site.js'))
        .pipe(gulp.dest(paths.scripts.dest));
}

export function styles() {
    return gulp.src(paths.styles.src)
        .pipe(sass())
        .pipe(concat('site.css'))
        .pipe(cleanCSS())
        // pass in options to the stream
        .pipe(rename({
            basename: 'site',
            suffix: '.min'
        }))
        .pipe(gulp.dest(paths.styles.dest));
}


export function scripts() {
    return gulp.src(paths.scripts.src, { sourcemaps: true })
        .pipe(babel())
        .pipe(concat('site.js'))
        .pipe(gulp.dest(paths.scripts.dest))
        .pipe(uglify())
        .pipe(concat('site.min.js'))
        .pipe(gulp.dest(paths.scripts.dest));
}

/*
 * You could even use `export as` to rename exported tasks
 */
function watchFiles() {
    gulp.watch(paths.scripts.src, scriptsDev);
    gulp.watch(paths.styles.src, stylesDev);
}
export { watchFiles as watch };

const build = gulp.series(clean, gulp.parallel(styles, scripts));
/*
 * Export a default task
 */
export default build;