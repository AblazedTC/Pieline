"use client"

import { useState } from "react"
import Link from "next/link"
import { useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Label } from "@/components/ui/label"
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group"
import { Checkbox } from "@/components/ui/checkbox"
import { Badge } from "@/components/ui/badge"
import { Pizza, ArrowLeft, ShoppingCart } from "lucide-react"
import type { Topping } from "@/lib/types"

const sizes = [
  { id: "small", name: 'Small (10")', price: 8.99 },
  { id: "medium", name: 'Medium (12")', price: 10.99 },
  { id: "large", name: 'Large (14")', price: 12.99 },
  { id: "xlarge", name: 'X-Large (16")', price: 14.99 },
]

const toppings: Topping[] = [
  { id: "1", name: "Pepperoni", price: 1.5, category: "meat" },
  { id: "2", name: "Italian Sausage", price: 1.5, category: "meat" },
  { id: "3", name: "Bacon", price: 1.5, category: "meat" },
  { id: "4", name: "Ham", price: 1.5, category: "meat" },
  { id: "5", name: "Grilled Chicken", price: 2.0, category: "meat" },
  { id: "6", name: "Mushrooms", price: 1.0, category: "vegetable" },
  { id: "7", name: "Bell Peppers", price: 1.0, category: "vegetable" },
  { id: "8", name: "Red Onions", price: 1.0, category: "vegetable" },
  { id: "9", name: "Black Olives", price: 1.0, category: "vegetable" },
  { id: "10", name: "Tomatoes", price: 1.0, category: "vegetable" },
  { id: "11", name: "Jalapeños", price: 1.0, category: "vegetable" },
  { id: "12", name: "Pineapple", price: 1.0, category: "vegetable" },
  { id: "13", name: "Extra Mozzarella", price: 1.5, category: "cheese" },
  { id: "14", name: "Parmesan", price: 1.5, category: "cheese" },
  { id: "15", name: "Feta", price: 2.0, category: "cheese" },
]

const sauces = [
  { id: "tomato", name: "Classic Tomato" },
  { id: "bbq", name: "BBQ Sauce" },
  { id: "white", name: "White Sauce" },
  { id: "pesto", name: "Pesto" },
]

export default function CustomPizzaPage() {
  const router = useRouter()
  const [selectedSize, setSelectedSize] = useState("medium")
  const [selectedSauce, setSelectedSauce] = useState("tomato")
  const [selectedToppings, setSelectedToppings] = useState<string[]>([])

  const toggleTopping = (toppingId: string) => {
    setSelectedToppings((prev) =>
      prev.includes(toppingId) ? prev.filter((id) => id !== toppingId) : [...prev, toppingId],
    )
  }

  const calculateTotal = () => {
    const sizePrice = sizes.find((s) => s.id === selectedSize)?.price || 0
    const toppingsPrice = selectedToppings.reduce((sum, toppingId) => {
      const topping = toppings.find((t) => t.id === toppingId)
      return sum + (topping?.price || 0)
    }, 0)
    return sizePrice + toppingsPrice
  }

  const handleAddToCart = () => {
    // In a real app, this would add to cart state/context
    router.push("/menu")
  }

  const groupedToppings = {
    meat: toppings.filter((t) => t.category === "meat"),
    vegetable: toppings.filter((t) => t.category === "vegetable"),
    cheese: toppings.filter((t) => t.category === "cheese"),
  }

  return (
    <div className="min-h-screen flex flex-col bg-background">
      {/* Header */}
      <header className="border-b border-border bg-card">
        <div className="container mx-auto px-4 py-4 flex items-center justify-between">
          <Link href="/" className="flex items-center gap-2">
            <Pizza className="h-8 w-8 text-primary" />
            <span className="text-2xl font-bold text-foreground">PieLine</span>
          </Link>
          <Button variant="ghost" asChild>
            <Link href="/menu">
              <ArrowLeft className="h-4 w-4 mr-2" />
              Back to Menu
            </Link>
          </Button>
        </div>
      </header>

      {/* Main Content */}
      <main className="flex-1 container mx-auto px-4 py-8">
        <div className="max-w-4xl mx-auto">
          <h1 className="text-4xl font-bold mb-2 text-foreground">Build Your Own Pizza</h1>
          <p className="text-muted-foreground mb-8 text-lg">Customize your perfect pizza exactly how you like it</p>

          <div className="grid lg:grid-cols-3 gap-6">
            {/* Customization Options */}
            <div className="lg:col-span-2 space-y-6">
              {/* Size Selection */}
              <Card>
                <CardHeader>
                  <CardTitle>Choose Your Size</CardTitle>
                  <CardDescription>Select the perfect size for your appetite</CardDescription>
                </CardHeader>
                <CardContent>
                  <RadioGroup value={selectedSize} onValueChange={setSelectedSize}>
                    <div className="space-y-3">
                      {sizes.map((size) => (
                        <div key={size.id} className="flex items-center space-x-3">
                          <RadioGroupItem value={size.id} id={size.id} />
                          <Label htmlFor={size.id} className="flex-1 cursor-pointer">
                            <div className="flex items-center justify-between">
                              <span>{size.name}</span>
                              <span className="text-muted-foreground">${size.price.toFixed(2)}</span>
                            </div>
                          </Label>
                        </div>
                      ))}
                    </div>
                  </RadioGroup>
                </CardContent>
              </Card>

              {/* Sauce Selection */}
              <Card>
                <CardHeader>
                  <CardTitle>Choose Your Sauce</CardTitle>
                  <CardDescription>Pick your base sauce</CardDescription>
                </CardHeader>
                <CardContent>
                  <RadioGroup value={selectedSauce} onValueChange={setSelectedSauce}>
                    <div className="grid grid-cols-2 gap-3">
                      {sauces.map((sauce) => (
                        <div key={sauce.id} className="flex items-center space-x-3">
                          <RadioGroupItem value={sauce.id} id={sauce.id} />
                          <Label htmlFor={sauce.id} className="cursor-pointer">
                            {sauce.name}
                          </Label>
                        </div>
                      ))}
                    </div>
                  </RadioGroup>
                </CardContent>
              </Card>

              {/* Toppings Selection */}
              <Card>
                <CardHeader>
                  <CardTitle>Add Toppings</CardTitle>
                  <CardDescription>Choose as many as you like</CardDescription>
                </CardHeader>
                <CardContent className="space-y-6">
                  {/* Meat Toppings */}
                  <div>
                    <h3 className="font-semibold mb-3 text-foreground">Meat</h3>
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                      {groupedToppings.meat.map((topping) => (
                        <div key={topping.id} className="flex items-center space-x-3">
                          <Checkbox
                            id={topping.id}
                            checked={selectedToppings.includes(topping.id)}
                            onCheckedChange={() => toggleTopping(topping.id)}
                          />
                          <Label htmlFor={topping.id} className="flex-1 cursor-pointer">
                            <div className="flex items-center justify-between">
                              <span>{topping.name}</span>
                              <span className="text-sm text-muted-foreground">+${topping.price.toFixed(2)}</span>
                            </div>
                          </Label>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* Vegetable Toppings */}
                  <div>
                    <h3 className="font-semibold mb-3 text-foreground">Vegetables</h3>
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                      {groupedToppings.vegetable.map((topping) => (
                        <div key={topping.id} className="flex items-center space-x-3">
                          <Checkbox
                            id={topping.id}
                            checked={selectedToppings.includes(topping.id)}
                            onCheckedChange={() => toggleTopping(topping.id)}
                          />
                          <Label htmlFor={topping.id} className="flex-1 cursor-pointer">
                            <div className="flex items-center justify-between">
                              <span>{topping.name}</span>
                              <span className="text-sm text-muted-foreground">+${topping.price.toFixed(2)}</span>
                            </div>
                          </Label>
                        </div>
                      ))}
                    </div>
                  </div>

                  {/* Cheese Toppings */}
                  <div>
                    <h3 className="font-semibold mb-3 text-foreground">Cheese</h3>
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                      {groupedToppings.cheese.map((topping) => (
                        <div key={topping.id} className="flex items-center space-x-3">
                          <Checkbox
                            id={topping.id}
                            checked={selectedToppings.includes(topping.id)}
                            onCheckedChange={() => toggleTopping(topping.id)}
                          />
                          <Label htmlFor={topping.id} className="flex-1 cursor-pointer">
                            <div className="flex items-center justify-between">
                              <span>{topping.name}</span>
                              <span className="text-sm text-muted-foreground">+${topping.price.toFixed(2)}</span>
                            </div>
                          </Label>
                        </div>
                      ))}
                    </div>
                  </div>
                </CardContent>
              </Card>
            </div>

            {/* Order Summary */}
            <div className="lg:col-span-1">
              <Card className="sticky top-24">
                <CardHeader>
                  <CardTitle>Your Pizza</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="space-y-2">
                    <div className="flex justify-between text-sm">
                      <span className="text-muted-foreground">Size:</span>
                      <span className="font-medium">{sizes.find((s) => s.id === selectedSize)?.name}</span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-muted-foreground">Sauce:</span>
                      <span className="font-medium">{sauces.find((s) => s.id === selectedSauce)?.name}</span>
                    </div>
                    {selectedToppings.length > 0 && (
                      <div className="pt-2 border-t border-border">
                        <p className="text-sm text-muted-foreground mb-2">Toppings:</p>
                        <div className="flex flex-wrap gap-1">
                          {selectedToppings.map((toppingId) => {
                            const topping = toppings.find((t) => t.id === toppingId)
                            return (
                              <Badge key={toppingId} variant="secondary" className="text-xs">
                                {topping?.name}
                              </Badge>
                            )
                          })}
                        </div>
                      </div>
                    )}
                  </div>
                  <div className="pt-4 border-t border-border">
                    <div className="flex justify-between items-center mb-4">
                      <span className="text-lg font-bold">Total:</span>
                      <span className="text-2xl font-bold text-primary">${calculateTotal().toFixed(2)}</span>
                    </div>
                    <Button className="w-full" size="lg" onClick={handleAddToCart}>
                      <ShoppingCart className="h-5 w-5 mr-2" />
                      Add to Cart
                    </Button>
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>
        </div>
      </main>
    </div>
  )
}
