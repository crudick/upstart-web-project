// Simple, reusable Tailwind class patterns
// Copy these patterns into your components - no complex logic needed!

// Form Container Patterns
export const FORM_CONTAINERS = {
  // Main form wrapper - centers form with nice spacing
  main: "min-h-screen bg-gray-50 flex items-center justify-center py-12 px-4",
  
  // Form content area - white card with shadow
  card: "bg-white shadow-lg rounded-lg p-8 w-full max-w-md",
  
  // Wide form for more fields
  cardWide: "bg-white shadow-lg rounded-lg p-8 w-full max-w-2xl",
}

// Text and Heading Patterns  
export const TEXT = {
  // Page title
  title: "text-2xl font-bold text-gray-900 mb-6 text-center",
  
  // Section headers within forms
  sectionHeader: "text-lg font-semibold text-gray-700 mb-4",
  
  // Success messages
  success: "text-green-600 bg-green-50 border border-green-200 rounded-lg p-4 mb-4",
  
  // Error messages  
  error: "text-red-600 bg-red-50 border border-red-200 rounded-lg p-4 mb-4",
}

// Input Field Patterns
export const INPUTS = {
  // Container for each field (label + input)
  fieldGroup: "mb-4",
  
  // Labels - consistent styling
  label: "block text-sm font-medium text-gray-700 mb-2",
  
  // Required field indicator
  required: "text-red-500",
  
  // Text inputs - focus states included
  text: "w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-colors",
  
  // Select dropdowns
  select: "w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500 bg-white transition-colors",
  
  // Textarea for longer text
  textarea: "w-full px-3 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-primary-500 focus:border-primary-500 transition-colors resize-vertical",
}

// Button Patterns
export const BUTTONS = {
  // Primary action button (submit, create, etc.)
  primary: "w-full bg-primary-600 text-white py-3 px-4 rounded-lg font-medium hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2 transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed",
  
  // Secondary button (cancel, back, etc.)
  secondary: "w-full bg-gray-200 text-gray-700 py-3 px-4 rounded-lg font-medium hover:bg-gray-300 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2 transition-colors",
  
  // Navigation/link style button
  link: "text-primary-600 hover:text-primary-700 font-medium transition-colors",
}

// Layout Patterns
export const LAYOUT = {
  // Navigation between forms
  nav: "flex justify-center space-x-4 mb-8",
  
  // Button groups (side by side buttons)
  buttonGroup: "flex space-x-4 mt-6",
  
  // Two column layout for forms
  twoColumn: "grid grid-cols-1 md:grid-cols-2 gap-4",
  
  // Three column layout
  threeColumn: "grid grid-cols-1 md:grid-cols-3 gap-4",
}

// Loading States
export const LOADING = {
  // Spinner/loading button state
  button: "opacity-50 cursor-not-allowed",
  
  // Loading text
  text: "text-gray-500 text-center py-4",
}

/* 
HOW TO USE THESE PATTERNS:

1. Import what you need:
   import { INPUTS, BUTTONS, FORM_CONTAINERS } from './styles';

2. Copy the class names directly into your JSX:
   <div className={FORM_CONTAINERS.main}>
     <form className={FORM_CONTAINERS.card}>
       <input className={INPUTS.text} />
       <button className={BUTTONS.primary}>Submit</button>
     </form>
   </div>

3. For new forms, just copy this basic structure and add your fields:
   - Use FORM_CONTAINERS.main for the page wrapper
   - Use FORM_CONTAINERS.card for the form itself  
   - Use INPUTS.fieldGroup for each field container
   - Use INPUTS.label for labels and INPUTS.text for inputs
   - Use BUTTONS.primary for submit buttons

4. Mix and match patterns as needed!
*/