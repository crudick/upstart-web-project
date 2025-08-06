/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
    "./public/index.html",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#effafb',
          100: '#d7f3f3',
          200: '#b3e9ea',
          300: '#81d9dc',
          400: '#46c2c7',
          500: '#00b1ac',
          600: '#009b97',
          700: '#00807b',
          800: '#00625f',
          900: '#0a4f4d',
        },
        gray: {
          50: '#f9fafb',
          100: '#f3f4f6',
          200: '#e5e7eb',
          300: '#d1d5db',
          400: '#9ca3af',
          500: '#6b7280',
          600: '#4b5563',
          700: '#374151',
          800: '#37465a',
          900: '#222929',
        },
        background: {
          light: '#effafb',
          card: '#ffffff',
        }
      },
      fontFamily: {
        'montserrat': ['Montserrat', 'sans-serif'],
      },
      borderRadius: {
        'upstart': '10px',
      },
      boxShadow: {
        'upstart': '0 2px 10px rgba(0, 0, 0, 0.1)',
        'upstart-lg': '0 4px 20px rgba(0, 0, 0, 0.1)',
      }
    },
  },
  plugins: [],
}

