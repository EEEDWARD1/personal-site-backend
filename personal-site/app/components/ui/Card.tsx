interface CardProps {
    children: React.ReactNode;
    className?: string;
}

export default function Card({
    children,
    className = "",
}: CardProps) {
    return (
        <div
            className={`
                rounded-2xl
                border
                border-gray-200
                bg-white
                shadow-sm
                p-6
                ${className}
            `}
        >
            {children}
        </div>
    );
}