<?php
try {
    $db = new PDO("mysql:host=hcwilli.at;dbname=d0424dc5", "d0424dc5", "3QHu9nnDesLrDbKF44vN");
    $db->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
} catch (PDOException $e) {
    die("Database connection failed: " . $e->getMessage());
}
echo $_SERVER['REQUEST_URI'];
$stmt = $db->prepare("SELECT * FROM country LIMIT 1,10");
$stmt->execute();
$items=$stmt->fetchAll(PDO::FETCH_ASSOC);
?>
<table>
<tbody>
    <?php foreach ($items as $item): ?>
        <tr>
             <?php foreach ($item as $key => $value): ?>
                <td><?php echo htmlspecialchars($value); ?></td>
            <?php endforeach; ?>
            <td>
                <a href="detail/<?php echo $item['id']; ?>">Detail</a>
            </td>
        </tr>   
    <?php endforeach; ?>
</tbody>
</table>
