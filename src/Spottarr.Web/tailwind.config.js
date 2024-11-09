/** @type {import('tailwindcss').Config} */
const defaultTheme = require('tailwindcss/defaultTheme')

module.exports = {
  content: [
    './**/*.{razor,html}'
  ],
  safelist: [
    {
      pattern: /bg-(sky|emerald|amber|red|fuchsia)/,
      variants: ['dark']
    },
  ],
  theme: {
    extend: {
      fontFamily: {
        'sans': ['"Open Sans"', ...defaultTheme.fontFamily.sans],
      },
    }
  },
  plugins: [],
}

