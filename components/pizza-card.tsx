"use client"

import Image from "next/image"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Plus } from "lucide-react"
import type { Pizza } from "@/lib/types"

interface PizzaCardProps {
  pizza: Pizza
  onAddToCart: (pizza: Pizza) => void
}

export function PizzaCard({ pizza, onAddToCart }: PizzaCardProps) {
  return (
    <Card className="overflow-hidden hover:shadow-lg transition-shadow">
      <div className="relative aspect-square overflow-hidden bg-muted">
        <Image src={pizza.image || "/placeholder.svg"} alt={pizza.name} fill className="object-cover" />
      </div>
      <CardHeader>
        <div className="flex items-start justify-between gap-2">
          <CardTitle className="text-xl">{pizza.name}</CardTitle>
          <Badge variant="secondary" className="shrink-0">
            ${pizza.basePrice.toFixed(2)}
          </Badge>
        </div>
        <CardDescription className="line-clamp-2">{pizza.description}</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="flex flex-wrap gap-1">
          {pizza.toppings?.slice(0, 4).map((topping, index) => (
            <Badge key={index} variant="outline" className="text-xs">
              {topping}
            </Badge>
          ))}
          {pizza.toppings && pizza.toppings.length > 4 && (
            <Badge variant="outline" className="text-xs">
              +{pizza.toppings.length - 4} more
            </Badge>
          )}
        </div>
      </CardContent>
      <CardFooter>
        <Button className="w-full" onClick={() => onAddToCart(pizza)}>
          <Plus className="h-4 w-4 mr-2" />
          Add to Cart
        </Button>
      </CardFooter>
    </Card>
  )
}
