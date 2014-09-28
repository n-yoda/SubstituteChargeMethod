set terminal png
set output "CircleErrors.png"
set logscale y
set xlabel "Count of charges."
set ylabel "Maximum error."
plot "CircleErrors.txt" with lines title "Circle"

