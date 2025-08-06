# Frontend Styling Guide

This project uses **Tailwind CSS** with a simple, reusable component system that makes it easy to create and modify forms without dealing with complex CSS.

## 🎯 **Quick Start - Copy These Patterns**

### Basic Form Structure
```jsx
import { INPUTS, BUTTONS, TEXT, LAYOUT } from '../styles';

function MyNewForm() {
  return (
    <div className="bg-white shadow-lg rounded-lg p-8 w-full max-w-2xl mx-auto">
      <h2 className={TEXT.title}>My Form Title</h2>
      <form onSubmit={handleSubmit}>
        
        {/* Section with multiple fields */}
        <div className="mb-6">
          <h3 className={TEXT.sectionHeader}>Section Name</h3>
          
          {/* Two fields side by side */}
          <div className={LAYOUT.twoColumn}>
            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>
                Field Name <span className={INPUTS.required}>*</span>
              </label>
              <input
                type="text"
                name="fieldName"
                className={INPUTS.text}
                required
              />
            </div>
            
            <div className={INPUTS.fieldGroup}>
              <label className={INPUTS.label}>Another Field</label>
              <select className={INPUTS.select}>
                <option value="">Select...</option>
                <option value="option1">Option 1</option>
              </select>
            </div>
          </div>
        </div>
        
        <button type="submit" className={BUTTONS.primary}>
          Submit
        </button>
      </form>
    </div>
  );
}
```

## 🏗️ **Available Style Patterns**

### Form Containers
- `FORM_CONTAINERS.main` - Full page background with centering
- `FORM_CONTAINERS.card` - White card for regular forms  
- `FORM_CONTAINERS.cardWide` - Wider card for complex forms

### Text Styles
- `TEXT.title` - Main page/form titles
- `TEXT.sectionHeader` - Section headers within forms
- `TEXT.success` - Green success messages
- `TEXT.error` - Red error messages

### Input Fields
- `INPUTS.fieldGroup` - Container for each label + input pair
- `INPUTS.label` - Consistent label styling
- `INPUTS.required` - Red asterisk for required fields
- `INPUTS.text` - Text inputs with focus states
- `INPUTS.select` - Dropdown selects
- `INPUTS.textarea` - Multi-line text areas

### Buttons
- `BUTTONS.primary` - Main action button (submit, create, etc.)
- `BUTTONS.secondary` - Secondary actions (cancel, back, etc.)
- `BUTTONS.link` - Link-style buttons

### Layouts
- `LAYOUT.twoColumn` - Two fields side by side
- `LAYOUT.threeColumn` - Three fields in a row
- `LAYOUT.nav` - Navigation button groups
- `LAYOUT.buttonGroup` - Multiple buttons side by side

## ✨ **Creating New Forms**

### Step 1: Copy the Basic Structure
Start with the form structure above and modify the fields you need.

### Step 2: Organize into Sections
Group related fields under section headers:
```jsx
<div className="mb-6">
  <h3 className={TEXT.sectionHeader}>Personal Information</h3>
  {/* Your fields here */}
</div>
```

### Step 3: Choose Your Layout
- Single column: Just use `INPUTS.fieldGroup`
- Two columns: Wrap fields in `<div className={LAYOUT.twoColumn}>`
- Three columns: Use `LAYOUT.threeColumn`

### Step 4: Add Fields
Each field follows this pattern:
```jsx
<div className={INPUTS.fieldGroup}>
  <label className={INPUTS.label}>
    Field Name {required && <span className={INPUTS.required}>*</span>}
  </label>
  <input
    type="text"
    name="fieldName"
    className={INPUTS.text}
    placeholder="Helpful placeholder"
  />
</div>
```

## 🎨 **Customization**

### Colors
The primary color is a nice blue. To change it, update `tailwind.config.js`:
```js
colors: {
  primary: {
    500: '#your-color', // Main color
    600: '#darker-shade', // Hover color
    // ... other shades
  }
}
```

### Spacing
All spacing uses Tailwind's standard scale:
- `mb-4` = 1rem margin bottom
- `mb-6` = 1.5rem margin bottom  
- `p-8` = 2rem padding all sides

### Responsive Design
Layouts automatically stack on mobile:
- `LAYOUT.twoColumn` becomes single column on small screens
- `max-w-2xl` limits form width on large screens

## 🔧 **Field Types Reference**

### Text Input
```jsx
<input type="text" className={INPUTS.text} />
```

### Number Input
```jsx
<input type="number" step="0.01" className={INPUTS.text} />
```

### Date Input
```jsx
<input type="date" className={INPUTS.text} />
```

### Select Dropdown
```jsx
<select className={INPUTS.select}>
  <option value="">Select...</option>
  <option value="value1">Option 1</option>
</select>
```

### Textarea
```jsx
<textarea rows={4} className={INPUTS.textarea} />
```

## 📱 **Mobile-First Design**

All forms are responsive by default:
- Forms stack vertically on mobile
- Touch-friendly button sizes
- Readable text at all screen sizes
- Proper spacing for thumb navigation

## 🚀 **Pro Tips**

1. **Always use sections** - Group related fields under `TEXT.sectionHeader`
2. **Consistent placeholder text** - Help users understand what to enter
3. **Mark required fields** - Use `<span className={INPUTS.required}>*</span>`
4. **Test on mobile** - Check how your forms look on small screens
5. **Copy existing patterns** - Look at `CreateUserForm.tsx` for examples

## 🤝 **Need Help?**

- Look at existing forms in the `/components` folder
- Check the `styles.ts` file for all available patterns
- Most styling is handled by the predefined classes - just copy and paste!