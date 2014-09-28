set terminal png
set output "SquareErrors.png"
set logscale y
set xlabel "Count of charges."
set ylabel "Maximum error."
plot "SquareErrors.txt" with lines title "Square"

