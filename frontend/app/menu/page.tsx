"use client"

import { useState } from "react"
import Link from "next/link"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Badge } from "@/components/ui/badge"
import { Pizza, Search, ShoppingCart, User } from "lucide-react"
import { PizzaCard } from "@/components/pizza-card"
import { CartSheet } from "@/components/cart-sheet"
import type { Pizza as PizzaType, CartItem } from "@/lib/types"

// Mock data for premade pizzas
const premadePizzas: PizzaType[] = [
  {
    id: "1",
    name: "Margherita",
    description: "Classic tomato sauce, fresh mozzarella, basil, and olive oil",
    basePrice: 12.99,
    image: "/margherita-pizza.png",
    category: "premade",
    size: "medium",
    toppings: ["mozzarella", "basil", "tomato sauce"],
  },
  {
    id: "2",
    name: "Pepperoni",
    description: "Tomato sauce, mozzarella, and premium pepperoni",
    basePrice: 14.99,
    image: "/pepperoni-pizza.png",
    category: "premade",
    size: "medium",
    toppings: ["mozzarella", "pepperoni", "tomato sauce"],
  },
  {
    id: "3",
    name: "Veggie Supreme",
    description: "Bell peppers, onions, mushrooms, olives, and tomatoes",
    basePrice: 13.99,
    image: "/vegetable-pizza.png",
    category: "premade",
    size: "medium",
    toppings: ["bell peppers", "onions", "mushrooms", "olives", "tomatoes"],
  },
  {
    id: "4",
    name: "BBQ Chicken",
    description: "BBQ sauce, grilled chicken, red onions, and cilantro",
    basePrice: 15.99,
    image: "/bbq-chicken-pizza.png",
    category: "premade",
    size: "medium",
    toppings: ["chicken", "bbq sauce", "red onions", "cilantro"],
  },
  {
    id: "5",
    name: "Hawaiian",
    description: "Tomato sauce, mozzarella, ham, and pineapple",
    basePrice: 14.99,
    image: "/hawaiian-pizza.png",
    category: "premade",
    size: "medium",
    toppings: ["mozzarella", "ham", "pineapple", "tomato sauce"],
  },
  {
    id: "6",
    name: "Meat Lovers",
    description: "Pepperoni, sausage, bacon, ham, and ground beef",
    basePrice: 16.99,
    image: "/meat-lovers-pizza.png",
    category: "premade",
    size: "medium",
    toppings: ["pepperoni", "sausage", "bacon", "ham", "ground beef"],
  },
]

export default function MenuPage() {
  const [searchQuery, setSearchQuery] = useState("")
  const [cartItems, setCartItems] = useState<CartItem[]>([])
  const [isCartOpen, setIsCartOpen] = useState(false)

  const filteredPizzas = premadePizzas.filter(
    (pizza) =>
      pizza.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      pizza.description.toLowerCase().includes(searchQuery.toLowerCase()),
  )

  const addToCart = (pizza: PizzaType) => {
    setCartItems((prev) => {
      const existingItem = prev.find((item) => item.pizza.id === pizza.id)
      if (existingItem) {
        return prev.map((item) => (item.pizza.id === pizza.id ? { ...item, quantity: item.quantity + 1 } : item))
      }
      return [
        ...prev,
        {
          id: `cart-${Date.now()}`,
          pizza,
          quantity: 1,
          price: pizza.basePrice,
        },
      ]
    })
    setIsCartOpen(true)
  }

  const cartItemCount = cartItems.reduce((sum, item) => sum + item.quantity, 0)

  return (
    <div className="min-h-screen flex flex-col bg-background">
      {/* Header */}
      <header className="sticky top-0 z-50 border-b border-border bg-card/95 backdrop-blur supports-[backdrop-filter]:bg-card/60">
        <div className="container mx-auto px-4 py-4 flex items-center justify-between">
          <Link href="/" className="flex items-center gap-2">
            <Pizza className="h-8 w-8 text-primary" />
            <span className="text-2xl font-bold text-foreground">PieLine</span>
          </Link>
          <div className="flex items-center gap-3">
            <Button variant="ghost" size="icon" asChild>
              <Link href="/account">
                <User className="h-5 w-5" />
              </Link>
            </Button>
            <Button variant="ghost" size="icon" className="relative" onClick={() => setIsCartOpen(true)}>
              <ShoppingCart className="h-5 w-5" />
              {cartItemCount > 0 && (
                <Badge className="absolute -top-1 -right-1 h-5 w-5 flex items-center justify-center p-0 text-xs">
                  {cartItemCount}
                </Badge>
              )}
            </Button>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="flex-1 container mx-auto px-4 py-8">
        {/* Search and Custom Pizza Button */}
        <div className="flex flex-col md:flex-row gap-4 mb-8">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-muted-foreground" />
            <Input
              type="search"
              placeholder="Search pizzas..."
              className="pl-10"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
          </div>
          <Button asChild size="lg" className="md:w-auto">
            <Link href="/menu/custom">Build Your Own Pizza</Link>
          </Button>
        </div>

        {/* Pizza Grid */}
        <div>
          <h2 className="text-3xl font-bold mb-6 text-foreground">Our Pizzas</h2>
          {filteredPizzas.length === 0 ? (
            <div className="text-center py-12">
              <p className="text-muted-foreground text-lg">No pizzas found matching your search.</p>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {filteredPizzas.map((pizza) => (
                <PizzaCard key={pizza.id} pizza={pizza} onAddToCart={addToCart} />
              ))}
            </div>
          )}
        </div>
      </main>

      {/* Cart Sheet */}
      <CartSheet
        isOpen={isCartOpen}
        onClose={() => setIsCartOpen(false)}
        cartItems={cartItems}
        setCartItems={setCartItems}
      />
    </div>
  )
}
