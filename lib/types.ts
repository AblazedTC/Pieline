export interface User {
  id: string
  name: string
  email: string
  phone: string
  role: "customer" | "employee" | "admin"
  addresses?: Address[]
  paymentMethods?: PaymentMethod[]
}

export interface Address {
  id: string
  street: string
  city: string
  state: string
  zipCode: string
  isDefault: boolean
}

export interface PaymentMethod {
  id: string
  type: "card" | "cash"
  last4?: string
  isDefault: boolean
}

export interface Pizza {
  id: string
  name: string
  description: string
  basePrice: number
  image: string
  category: "premade" | "custom"
  toppings?: string[]
  size?: "small" | "medium" | "large" | "xlarge"
}

export interface Topping {
  id: string
  name: string
  price: number
  category: "meat" | "vegetable" | "cheese" | "sauce"
}

export interface CartItem {
  id: string
  pizza: Pizza
  quantity: number
  customizations?: {
    size: "small" | "medium" | "large" | "xlarge"
    toppings: Topping[]
  }
  price: number
}

export interface Order {
  id: string
  userId: string
  items: CartItem[]
  subtotal: number
  tax: number
  deliveryFee: number
  discount: number
  total: number
  status: "pending" | "preparing" | "ready" | "delivered" | "cancelled"
  deliveryAddress: Address
  paymentMethod: PaymentMethod
  createdAt: Date
  updatedAt: Date
}

export interface Coupon {
  code: string
  discount: number
  type: "percentage" | "fixed"
  minOrder?: number
  expiresAt?: Date
}
