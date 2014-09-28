set terminal png
set output "CircleDistErrors64.png"
set logscale y
set xlabel "Distance."
set ylabel "Maximum error."
plot "CircleDistErrors.txt" with lines title "Circle n = 64"

