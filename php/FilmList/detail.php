<?php
try {
    $db = new PDO("mysql:host=hcwilli.at;dbname=d0424dc5", "d0424dc5", "3QHu9nnDesLrDbKF44vN");
    $db->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
} catch (PDOException $e) {
    die("Database connection failed: " . $e->getMessage());
}
echo $_SERVER['REQUEST_URI'];
$stmt = $db->prepare("SELECT * FROM film_list LIMIT 1,1");
$stmt->execute();
$items=$stmt->fetchAll(PDO::FETCH_ASSOC);
?>
<dl>
    <?php foreach ($items as $item): ?>
             <?php foreach ($item as $key => $value): ?>
                <dt><?php echo htmlspecialchars($key); ?></dt><dd><?php echo htmlspecialchars($value); ?></dd>
            <?php endforeach; ?>
    <?php endforeach; ?>
</dl>
